using Application.AppLogger;
using Application.DataServices;
using Application.Pipelines.NO.Collection.DriverAndHorsesStep;
using Models.DbModels;
using Models.DbModels.Updates;
using Models.ScrapeData;
using Models.Settings;
using Scraping.Errors;
using Scraping.Processors;
using Scraping.Spider.NO;
using ShellProgressBar;

namespace Application.Pipelines.NO.Steps;

public class DriverAndHorseStep(
    BrowserOptions browserOptions,
    ScraperSettings scraperSettings,
    IDataServiceRegistry dataServices,
    IBufferDataService bufferService)
{
    private const int DriverBatchSize = 4;
    private const int HorseBatchSize = 16;
    private const int DbActionBatchSize = 200;
    
    private readonly HashSet<string> _driversToCollect = [];
    private readonly HashSet<string> _horsesToCollect = [];
    
    private readonly List<Driver> _newDrivers = [];
    private readonly List<Horse> _newHorses = [];

    private readonly List<ResultScrapeData> _driverResultsScrapeData = [];
    private readonly List<ResultScrapeData> _horseResultsScrapeData = [];
    
    private readonly RaceDataContainer _raceDataContainer = new();
    private DriverAndHorseStateDataReport _dataReport = new();
    
    public async Task RunAsync()
    {
        await ResolveDataToCollect();
        

        // clear driver and horse buffer here
        while (_driversToCollect.Count > 0 || _horsesToCollect.Count > 0)
        {
            var now = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            AppLogger.AppLogger.LogSubheader($"Resolving drivers : ({_driversToCollect.Count} / {bufferService.DriverBuffer.Count}) " +
                                             $"| horses: ({_horsesToCollect.Count} / {bufferService.HorseBuffer.Count}) " +
                                             $"| {now}");
            
            // process data buffers
            await ProcessDriverBuffer();
            await ProcessHorseBuffer();

            // store driver and horse data
            await SaveDriverHorseData();
            
            // process collected race data
            await ProcessDriverResults();
            await ProcessHorseResults();
            
            // store collected race data
            await StoreRaceData();

            // find unregistered drivers and horses to add 
            await ResolveDataToCollect();
            
            _dataReport.Report();
            _raceDataContainer.Clear();
        }
    }

    /// <summary>
    /// Save collected driver and horse data
    /// </summary>
    private async Task SaveDriverHorseData()
    {
        _dataReport.NewDrivers += _newDrivers.Count;
        _dataReport.NewHorses += _newHorses.Count;

        var count = _newDrivers.Count + _newHorses.Count;
        var message = "Saving new drivers and horses to entity";
        var options = CreateProgressBarOptions();
        using var bar = new ProgressBar(count, message, options);
        
        foreach (var d in _newDrivers)
        {
            await dataServices.DriverDataService.AddAsync(d);
            bar.Tick();
        }

        foreach (var h in _newHorses)
        {
            await dataServices.HorseDataService.AddAsync(h);
            bar.Tick();
        }
        
        _newDrivers.Clear();
        _newHorses.Clear();
    }


    /// <summary>
    /// Iterate driver buffer to gather data. Will clear buffer when complete
    /// </summary>
    private async Task ProcessDriverBuffer()
    {
        if (_driversToCollect.Count == 0) return;
        
        var message = "Collecting Driver data";
        var options = CreateProgressBarOptions();
        using var bar = new ProgressBar(_driversToCollect.Count, message, options);

        using var semaphore = new SemaphoreSlim(2);
        var tasks = new List<Task>();
        
        foreach (var d in _driversToCollect)
        {
            tasks.Add(RunProcessDriverBufferTask(d, semaphore, bar));
        }
        
        await Task.WhenAll(tasks);
    }
    /// <summary>
    /// Run CollectDriverData as semaphore task
    /// </summary>
    private async Task RunProcessDriverBufferTask(string sourceId, SemaphoreSlim semaphore, ProgressBar bar)
    {
        await semaphore.WaitAsync();
        try
        {
            var tries = 0;
            var treshhold = 3;

            while (tries < treshhold)
            {
                try
                {
                    await CollectDriverData(sourceId);
                    bar.Tick();
                    break;
                }
                catch (NoPanelButtonException)
                {
                    if (++tries > treshhold)
                    {
                        await FileLogger.AddToDriverNoPanel(sourceId);
                        await bufferService.RemoveDriverAsync(sourceId);
                        bar.Tick();
                        break;
                    }
                }
                catch (YearSelectNotFoundException ex)
                {
                    await FileLogger.AddToDriverNoYearSelect(sourceId);
                    await bufferService.RemoveDriverAsync(sourceId);
                    bar.Tick();
                    break;
                }
            }
        }
        finally
        {
            semaphore.Release();
        }
    }
    /// <summary>
    /// Collect driver data from browser!
    /// </summary>
    private async Task CollectDriverData(string driverSourceId)
    {
        var bot = new DriverBotNo(browserOptions, scraperSettings, driverSourceId);
        await bot.RunBrowser(bot.Execute);
        _driverResultsScrapeData.AddRange(bot.RaceDataCollected);

        var processor = new ProcessDriverScrapeData();
        var newDriver = processor.Process(bot.DriverDataCollected);
        
        // resolve licence for driver
        var driverLicenceExists = dataServices.DriverLicenseDataService
            .CheckExists(processor.NewDriverLicense.Code);
        if (driverLicenceExists)
        {
            var licence = dataServices.DriverLicenseDataService.GetModel(processor.NewDriverLicense.Code);
            newDriver.DriverLicenseId = licence.Id;
        }
        else
        {
            await dataServices.DriverLicenseDataService.AddAsync(processor.NewDriverLicense);
            newDriver.DriverLicenseId = processor.NewDriverLicense.Id;
            _dataReport.NewDriverLicenses++;
        }
        
        _driverResultsScrapeData.AddRange(bot.RaceDataCollected);
        
        
        var driverFromDb = await dataServices.DriverDataService.GetDriverFromDb(bot.DriverDataCollected.SourceId);
        if (driverFromDb.Count > 0) return;  
        
        _newDrivers.Add(newDriver);
    }
    
    
    /// <summary>
    /// Iterate horse buffer to gather data. Will clear buffer when complete.
    /// </summary>
    private async Task ProcessHorseBuffer()
    {
        if (_horsesToCollect.Count == 0) return;
        
        var message = "Collecting Horse data";
        var options = CreateProgressBarOptions();
        using var bar = new ProgressBar(_horsesToCollect.Count, message, options);
        
        using var semaphore = new SemaphoreSlim(2);
        var tasks = new List<Task>();
        
        foreach (var h in _horsesToCollect)
        {
            tasks.Add(RunProcessHorseBufferTask(h, semaphore, bar));
        }
        await Task.WhenAll(tasks); 
    }
    /// <summary>
    /// Run CollectHorseData as semaphore task 
    /// </summary>
    private async Task RunProcessHorseBufferTask(string sourceId, SemaphoreSlim semaphore, ProgressBar bar)
    {
        await semaphore.WaitAsync();
        try
        {
            var complete = false;
            var tries = 0;
            var treshhold = 5;
            while (!complete)
            {
                try
                {
                    await CollectHorseData(sourceId);
                    complete = true;
                    bar.Tick();
                }
                catch (NoPanelButtonException)
                {
                    if (++tries > treshhold)
                    {
                        await FileLogger.AddToHorseNoPanel(sourceId);
                        await bufferService.RemoveHorseAsync(sourceId);
                        complete = true;
                        bar.Tick();
                    }
                }
            }
        }
        finally
        {
            semaphore.Release();
        }
    }
    /// <summary>
    /// Collect horse data from browser 
    /// </summary>
    private async Task CollectHorseData(string horseSourceId)
    {
        var bot = new HorseBotNo(browserOptions, scraperSettings, horseSourceId);
        await bot.RunBrowser(bot.Execute);
        _horseResultsScrapeData.AddRange(bot.RaceDataCollected);
        
        var horseFromDb = await dataServices.HorseDataService.GetHorseFromDb(horseSourceId);
        if (horseFromDb.Count > 0) return;

        var processor = new ProcessHorseScrapeData();
        var newHorse = processor.Process(bot.HorseDataCollected);
        _newHorses.Add(newHorse);
        _horseResultsScrapeData.AddRange(bot.RaceDataCollected);
    }
    
    
    /// <summary>
    /// Iterate collected driver data for processing. Adds to create and update lists
    /// </summary>
    private async Task ProcessDriverResults()
    {
        if (_driverResultsScrapeData.Count == 0) return;
        
        var message = "Processing collected driver results";
        var options = CreateProgressBarOptions();
        using var bar = new ProgressBar(_driverResultsScrapeData.Count, message, options);
        foreach (var item in _driverResultsScrapeData)
        {
            var id = dataServices.DriverDataService.GetModel(item.DriverSourceId).Id;
            var data = await ProcessResultData(item, driverId: id);
            
            switch (data.Option)
            {
                case CreateUpdateOptions.Create:
                    _raceDataContainer.StartNumbersCreate.Add(data.StartNumberData);
                    _dataReport.StartnumberCreated++;
                    break;
                case CreateUpdateOptions.Update:
                    _raceDataContainer.StartNumbersUpdateDriver.Add(new RaceStartNumberUpdateDriver
                    {
                        Id = data.StartNumberData.Id,
                        DriverId = data.StartNumberData.DriverId
                    });
                    _dataReport.StartnumberUpdatedDriver++;
                    break;
                case CreateUpdateOptions.Ignore:
                    break;
            }
            bar.Tick();
        }
        _driverResultsScrapeData.Clear();
    }
    /// <summary>
    /// Iterate collected horse data for processing. Adds to create ad update lists
    /// </summary>
    private async Task ProcessHorseResults()
    {
        if (_horseResultsScrapeData.Count == 0) return;
        
        var message = "Processing collected horse results";
        var options = CreateProgressBarOptions();
        using var bar = new ProgressBar(_horseResultsScrapeData.Count, message, options);
        foreach (var item in _horseResultsScrapeData)
        {
            var id = dataServices.HorseDataService.GetModel(item.HorseSourceId).Id;
            var data = await ProcessResultData(item, horseId: id);
            
            switch (data.Option)
            {
                case CreateUpdateOptions.Create:
                    _raceDataContainer.StartNumbersCreate.Add(data.StartNumberData);
                    _dataReport.StartnumberCreated++;
                    break;
                case CreateUpdateOptions.Update:
                    _raceDataContainer.StartNumbersUpdateHorse.Add(new RaceStartNumberUpdateHorse
                    {
                        Id = data.StartNumberData.Id,
                        HorseId = data.StartNumberData.HorseId
                    });
                    _dataReport.StartnumberUpdatedHorses++;
                    break;
                case CreateUpdateOptions.Ignore:
                    break;
            }
            bar.Tick();
        }
        _horseResultsScrapeData.Clear();
    }
    /// <summary>
    /// Process the collected data and resolve if create or update
    /// </summary>
    private async Task<CreateUpdateResult> ProcessResultData(ResultScrapeData resultData, string driverId = "", string horseId = "")
    {
        // add to buffers if data not collected!!!
        await bufferService.AddDriverAsync(resultData.DriverSourceId);
        await bufferService.AddHorseAsync(resultData.HorseSourceId);
        
        // Normalize racecourse name and ensure exists in database
        var racecourseNorm = resultData.RaceCourse.ToUpper();
        var racecourseExists = dataServices.RaceCourseDataService.CheckExists(racecourseNorm);

        if (!racecourseExists)
        {
            await dataServices.RaceCourseDataService.AddAsync(new Racecourse
            {
                Id = Guid.NewGuid().ToString(),
                Name = racecourseNorm
            });
            _dataReport.NewRacecourses++;
        }
        var racecourseId = dataServices.RaceCourseDataService.GetModel(racecourseNorm).Id;
        
        
        // get or create competition data item
        var date = FormatDateString(resultData.Date);
        var competitionKey = $"{racecourseId}_{date}";
        
        var competitionDbExists = dataServices.CompetitionDataService.CheckExists(competitionKey);
        var competitionTempExists = _raceDataContainer.Competitions.ContainsKey(competitionKey);

        if (!competitionDbExists && !competitionTempExists)
        {
            _raceDataContainer.Competitions.Add(competitionKey, new Competition
            {
                Id = Guid.NewGuid().ToString(),
                RaceCourseId = racecourseId,
                Date = date
            });
            _dataReport.NewCompetition++;
        }
        
        var competitionId = competitionDbExists ? 
            dataServices.CompetitionDataService.GetModel(competitionKey).Id : 
            _raceDataContainer.Competitions[competitionKey].Id;
        
        
        // get or create race data item
        var raceNumber = int.Parse(resultData.RaceNumber);
        var raceKey = $"{competitionId}_{raceNumber}";
        
        var raceDbExists = dataServices.RaceDataService.CheckExists(raceKey);
        var raceTempExists = _raceDataContainer.Races.ContainsKey(raceKey);

        if (!raceDbExists && !raceTempExists)
        {
            _raceDataContainer.Races.Add(raceKey, new Race
            {
                Id = Guid.NewGuid().ToString(),
                CompetitionId = competitionId,
                RaceNumber = raceNumber,
                Distance = int.TryParse(resultData.Distance, out var d1) ? d1 : -1
            });
            _dataReport.NewRaces++;
        }
        
        var raceId = raceDbExists ? dataServices.RaceDataService.GetModel(raceKey).Id :
            _raceDataContainer.Races[raceKey].Id;
        
        
        // get or create race start number item
        var programNumber = int.Parse(resultData.StartNumber);
        var raceStartNumberKey = $"{raceId}_{programNumber}";
        
        var raceStartNumberInDbExists = dataServices.RaceStartNumberDataService.CheckExists(raceStartNumberKey);
        var raceStartNumberTempExists = _raceDataContainer.AllStartNumbers.ContainsKey(raceStartNumberKey);
        
        var raceStartNumber = raceStartNumberInDbExists ? 
            dataServices.RaceStartNumberDataService.GetModel(raceStartNumberKey) :
            (raceStartNumberTempExists ? _raceDataContainer.AllStartNumbers[raceStartNumberKey] : 
                new RaceStartNumber
                {
                    Id = Guid.NewGuid().ToString(),
                    RaceId = raceId,
                    ProgramNumber = programNumber,
                    TrackNumber = int.Parse(resultData.TrackNumber),
                    Distance = int.Parse(resultData.Distance),
                    ForeShoe = ResolveForeShoe(resultData.ForeShoe),
                    HindShoe = ResolveHindShoe(resultData.HindShoe),
                    Cart = resultData.Cart,
                    FromDirectSource = false
                });
        
        // resolve race startnumber here
        var raceResultInDbExists = dataServices.RaceResultDataService.CheckExists(raceStartNumber.Id);
        var raceResultTempExists = _raceDataContainer.AllStartNumbers.ContainsKey(raceStartNumber.Id);
        
        if (!raceResultInDbExists && !raceResultTempExists)
        {
            var data = new RaceResult
            {
                Id = Guid.NewGuid().ToString(),
                RaceStartNumberId = raceStartNumber.Id,
                Place = int.TryParse(resultData.Place, out var p) ? p : 0,
                Time = resultData.Time,
                Odds = int.TryParse(resultData.Odds, out var od) ? od : 0,
                KmTime = resultData.KmTime,
                RRemark = resultData.RRemark,
                GRemark = resultData.GRemark,
                FromDirectSource = false
            };
            _raceDataContainer.RaceResultsCreate.Add(data);
            _raceDataContainer.RaceResultKeys.Add(raceStartNumber.Id);
        }
        
        if (driverId != "")
            if (raceStartNumber.Id == driverId)
                return new CreateUpdateResult
                {
                    Option = CreateUpdateOptions.Ignore
                };
            else raceStartNumber.DriverId = driverId;
        if (horseId != "")
            if (raceStartNumber.Id == horseId)
                return new CreateUpdateResult
                {
                    Option = CreateUpdateOptions.Ignore
                };
            else raceStartNumber.HorseId = horseId;
        
        return new CreateUpdateResult
        {
            StartNumberData = raceStartNumber,
            Option = (raceResultInDbExists || raceResultTempExists)
                ? CreateUpdateOptions.Update
                : CreateUpdateOptions.Create
        };
    }
    
    
    /// <summary>
    /// Handle lists for create and update. Will clear create and update lists
    /// </summary>
    private async Task StoreRaceData()
    {

        var compChunks = _raceDataContainer.Competitions.Values.Chunk(DbActionBatchSize).ToList();
        var raceChunks = _raceDataContainer.Races.Values.Chunk(DbActionBatchSize).ToList();
        var rsnCreateChunks = _raceDataContainer.StartNumbersCreate.Chunk(DbActionBatchSize).ToList();
        var rsnDriverChunks = _raceDataContainer.StartNumbersUpdateDriver.Chunk(DbActionBatchSize).ToList();
        var rsnHorseChunks = _raceDataContainer.StartNumbersUpdateHorse.Chunk(DbActionBatchSize).ToList();
        var resultChunks = _raceDataContainer.RaceResultsCreate.Chunk(DbActionBatchSize).ToList();
        
        var count = compChunks.Count + raceChunks.Count + rsnCreateChunks.Count +
                    rsnDriverChunks.Count + rsnHorseChunks.Count + resultChunks.Count;
        var message = "Storing data to database";
        var options = CreateProgressBarOptions();
        using var bar = new ProgressBar(count,message,options);
        
        bar.Message = "Inserting new Competitions";
        foreach (var c in compChunks)
        {
            await dataServices.CompetitionDataService.AddBulkAsync(c.ToList());
            bar.Tick();
        }

        bar.Message = "Inserting new races";
        foreach (var r in raceChunks)
        {
            await dataServices.RaceDataService.AddBulkAsync(r.ToList());
            bar.Tick();
        }

        bar.Message = "Inserting new start numbers";
        foreach (var rsnCreate in rsnCreateChunks)
        {
            await dataServices.RaceStartNumberDataService.BulkAddAsync(rsnCreate.ToList());
            bar.Tick();
        }

        bar.Message = "Updating drivers in start numbers";
        foreach (var rsnDriver in rsnDriverChunks)
        {
            await dataServices.RaceStartNumberDataService.BulkUpdateDriversAsync(rsnDriver.ToList());
            bar.Tick();
        }
        
        bar.Message = "Updating horses in start numbers";
        foreach (var rsnHorse in rsnHorseChunks)
        {
            await dataServices.RaceStartNumberDataService.BulkUpdateHorsesAsync(rsnHorse.ToList());
            bar.Tick();
        }

        foreach (var res in resultChunks)
        {
            await dataServices.RaceResultDataService.AddBulkAsync(res.ToList());
            bar.Tick();
        }
    }
    /// <summary>
    /// Iterate collected result data and updates both driver and horse buffers;
    /// Will clear result data
    /// </summary>
    private async Task ResolveDataToCollect()
    {
        
        var driverTreshhold = 0;
        var horseTreshhold = 0;
        
        foreach (var driver in _driversToCollect)
        {
            // fjern fra buffer
            await bufferService.RemoveDriverAsync(driver);
            // legg til i drive cache
            await dataServices.DriverDataService.AddDriverToCacheAsync(driver);
        }

        foreach (var horse in _horsesToCollect)
        {
            // fjern fra buffer
            await bufferService.RemoveHorseAsync(horse);
            // legg til i horses cache
            await dataServices.HorseDataService.AddHorseToCacheAsync(horse);
        }
        
        _driversToCollect.Clear();
        _horsesToCollect.Clear();

        foreach (var driver in bufferService.DriverBuffer)
        {
            if (++driverTreshhold > DriverBatchSize) break;
            _driversToCollect.Add(driver);
        }

        foreach (var horse in bufferService.HorseBuffer)
        {
            if (++horseTreshhold > HorseBatchSize) break;
            _horsesToCollect.Add(horse);
        }
    }
    
    private bool? ResolveForeShoe(string shoe)
    {
        if (shoe == "showForeShoeOn") return true;
        if (shoe == "showForeShoeOff") return false;
        return null;
    }
    private bool? ResolveHindShoe(string shoe)
    {
        if (shoe == "showHindShoeOn") return true;
        if (shoe == "showHindShoeOff") return false;
        return null;
    }

    private string FormatDateString(string date)
    {
        var split = date.Split(".").Reverse();
        return string.Join("-", split);
    }
    private ProgressBarOptions CreateProgressBarOptions()
    {
        return new ProgressBarOptions
        {
            ForegroundColor = ConsoleColor.DarkCyan,
            BackgroundColor = ConsoleColor.White,
            ForegroundColorDone = ConsoleColor.DarkCyan
        };
    }
}