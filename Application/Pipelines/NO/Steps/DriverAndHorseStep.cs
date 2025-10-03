using Application.DataServices.Interfaces;
using Application.Pipelines.NO.Collection.DriverAndHorsesStep;
using Models.DbModels;
using Models.DbModels.Updates;
using Models.ScrapeData;
using Models.Settings;
using Scraping.Processors;
using Scraping.Spider.NO;
using ShellProgressBar;

namespace Application.Pipelines.NO.Steps;

public class DriverAndHorseStep(
    BrowserOptions browserOptions,
    ScraperSettings scraperSettings,
    IDataServiceCollection dataServices,
    HashSet<string> driverBuffer,
    HashSet<string> horseBuffer)
{
    private const int DriverBatchSize = 2;
    private const int HorseBatchSize = 6;
    
    private readonly HashSet<string> _driverExtract = [];
    private readonly HashSet<string> _horseExtract = [];
    
    private readonly List<Driver> _newDrivers = [];
    private readonly List<Horse> _newHorses = [];

    private readonly List<ResultScrapeData> _driverResultsScrapeData = [];
    private readonly List<ResultScrapeData> _horseResultsScrapeData = [];
    
    private readonly List<RaceStartNumber> _startNumbersToCreate = [];
    private readonly List<RaceStartNumberUpdateDriver> _startNumbersUpdateDriver = [];
    private readonly List<RaceStartNumberUpdateHorse> _startNumbersUpdateHorse = [];
    private readonly List<RaceResult> _raceResultsToCreate = [];
    
    private DriverAndHorseStateDataReport _dataReport = new DriverAndHorseStateDataReport();
    
    public async Task RunAsync()
    {
        ResolveExtractData();
        

        // clear driver and horse buffer here
        while (_driverExtract.Count > 0 || _horseExtract.Count > 0)
        {
            AppLogger.LogSubheader($"Resolving drivers : ({_driverExtract.Count} / {driverBuffer.Count}) | horses: ({_horseExtract.Count} / {horseBuffer.Count})");
            
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
            ResolveExtractData();
            
            _dataReport.Report();
            _driverExtract.Clear();
            _horseExtract.Clear();
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
        if (_driverExtract.Count == 0) return;
        
        var message = "Collecting Driver data";
        var options = CreateProgressBarOptions();
        using var bar = new ProgressBar(_driverExtract.Count, message, options);
        
        using var semaphore = new SemaphoreSlim(DriverBatchSize);
        var tasks = new List<Task>();
        
        foreach (var d in _driverExtract)
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
        var complete = false;
        await semaphore.WaitAsync();
        try
        {
            while (!complete)
            {
                try
                {
                    await CollectDriverData(sourceId);
                    bar.Tick();
                    complete = true;
                }
                catch (TimeoutException)
                {
                    AppLogger.LogNegative($"Error occured when collecting data for driver {sourceId}");
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

        var driverFromDb = await dataServices.DriverDataService.GetDriverFromDb(bot.DriverDataCollected.SourceId);
        if (driverFromDb.Count > 0) return;  
        
        var dl = dataServices.DriverLicenseDataService.GetFullCache();
        var processor = new ProcessDriverScrapeData(dl);
        var newDriver = processor.Process(bot.DriverDataCollected);
        _driverResultsScrapeData.AddRange(bot.RaceDataCollected);
        
        if (processor.NewDriverLicense != null)
        {
            var dlExists = dataServices.DriverLicenseDataService.CheckExists(processor.NewDriverLicense.Code);
            switch (dlExists)
            {
                case false:
                    await dataServices.DriverLicenseDataService.AddAsync(processor.NewDriverLicense);
                    _dataReport.NewDriverLicenses++;
                    break;
                case true:
                {
                    var dlId = dataServices.DriverDataService.GetModel(processor.NewDriverLicense.Code).Id;
                    newDriver.DriverLicenseId = dlId;
                    break;
                }
            }
        }
        _newDrivers.Add(newDriver);

    }
    
    
    /// <summary>
    /// Iterate horse buffer to gather data. Will clear buffer when complete.
    /// </summary>
    private async Task ProcessHorseBuffer()
    {
        if (_horseExtract.Count == 0) return;
        
        var message = "Collecting Horse data";
        var options = CreateProgressBarOptions();
        using var bar = new ProgressBar(_horseExtract.Count, message, options);
        
        using var semaphore = new SemaphoreSlim(DriverBatchSize);
        var tasks = new List<Task>();
        
        foreach (var h in _horseExtract)
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
        var complete = false;
        await semaphore.WaitAsync();
        try
        {
            while (!complete)
            {
                try
                {
                    await CollectHorseData(sourceId);
                    bar.Tick();
                    complete = true;
                }
                catch (TimeoutException)
                {
                    AppLogger.LogNegative($"Error occured when collecting data for horse {sourceId}");
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
            if (data.Option == CreateUpdateOptions.Ignore)
            {
                bar.Tick();
                continue;
            }
            
            switch (data.Option)
            {
                case CreateUpdateOptions.Create:
                    _startNumbersToCreate.Add(data.StartNumberData);
                    _dataReport.StartnumberCreated++;
                    break;
                case CreateUpdateOptions.Update:
                    _startNumbersUpdateDriver.Add(new RaceStartNumberUpdateDriver
                    {
                        Id = data.StartNumberData.DriverId,
                        DriverId = data.StartNumberData.DriverId,
                    });
                    _dataReport.StartnumberUpdatedDriver++;
                    break;
            }
            bar.Tick();
        }
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
            if (data.Option == CreateUpdateOptions.Ignore)
            {
                bar.Tick();
                continue;
            };           
            
            switch (data.Option)
            {
                case CreateUpdateOptions.Create:
                    _startNumbersToCreate.Add(data.StartNumberData);
                    _dataReport.StartnumberCreated++;
                    break;
                case CreateUpdateOptions.Update:
                    _startNumbersUpdateHorse.Add(new RaceStartNumberUpdateHorse
                    {
                        Id = data.StartNumberData.Id,
                        HorseId = data.StartNumberData.HorseId,
                    });
                    _dataReport.StartnumberUpdatedHorses++;
                    break;
                case CreateUpdateOptions.Ignore: break;
            }
            bar.Tick();
        }
    }
    /// <summary>
    /// Process the collected data and resolve if create or update
    /// </summary>
    private async Task<CreateUpdateResult> ProcessResultData(ResultScrapeData resultData, string driverId = "", string horseId = "")
    {
        
        // add driver and horse to buffer if not previously resolved
        var driverExists = dataServices.DriverDataService.CheckExists(resultData.DriverSourceId);
        var horseExists = dataServices.HorseDataService.CheckExists(resultData.HorseSourceId);
        if (!driverExists) driverBuffer.Add(resultData.DriverSourceId);
        if (!horseExists) horseBuffer.Add(resultData.HorseSourceId);
        
        // Normalize racecourse name and ensure exists in database
        var racecourseNorm = resultData.RaceCourse.ToUpper();
        var racecourseExists = dataServices.RaceCourseDataService.CheckExists(racecourseNorm);

        if (!racecourseExists)
        {
            await dataServices.RaceCourseDataService.AddAsync(new Racecourse()
            {
                Id = Guid.NewGuid().ToString(),
                Name = racecourseNorm,
            });
            _dataReport.NewRacecourses++;
        }
        var racecourseId = dataServices.RaceCourseDataService.GetModel(racecourseNorm).Id;
        
        
        // get or create competition data item
        var date = FormatDateString(resultData.Date);
        var competitionKey = $"{racecourseId}_{date}";
        var competitionExists = dataServices.CompetitionDataService.CheckExists(competitionKey);
        if (!competitionExists)
        {
            await dataServices.CompetitionDataService.AddAsync(new Competition()
            {
                Id = Guid.NewGuid().ToString(),
                RaceCourseId = racecourseId,
                Date = date
            });
            _dataReport.NewCompetition++;
        }
        var competitionId = dataServices.CompetitionDataService.GetModel(competitionKey).Id;
        
        
        // get or create race data item
        var raceNumber = int.Parse(resultData.RaceNumber);
        var raceKey = $"{competitionId}_{raceNumber}";
        var race = dataServices.RaceDataService.CheckExists(raceKey);
        if (!race)
        {
            await dataServices.RaceDataService.AddAsync(new Race()
            {
                Id = Guid.NewGuid().ToString(),
                CompetitionId = competitionId,
                RaceNumber = raceNumber,
                Distance = int.TryParse(resultData.Distance, out var d1) ? d1 : -1,
            });
            _dataReport.NewRaces++;
        }
        var raceId = dataServices.RaceDataService.GetModel(raceKey).Id;

        
        // get or create race start number item
        var programNumber = int.Parse(resultData.StartNumber);
        var raceStartNumberKey = $"{raceId}_{programNumber}";
        var raceStartNumberExists = dataServices.RaceStartNumberDataService.CheckExists(raceStartNumberKey);
        var raceStartNumber = raceStartNumberExists
            ? dataServices.RaceStartNumberDataService.GetModel(raceStartNumberKey)
            : new RaceStartNumber()
            {
                Id = Guid.NewGuid().ToString(),
                RaceId = raceId,
                ProgramNumber = programNumber,
                TrackNumber = int.Parse(resultData.TrackNumber),
                Distance = int.Parse(resultData.Distance),
                ForeShoe = ResolveForeShoe(resultData.ForeShoe),
                HindShoe = ResolveHindShoe(resultData.HindShoe),
                Cart = resultData.Cart,
                FromDirectSource = false,
            };
        
        if (driverId != "")
            if (raceStartNumber.DriverId == driverId)
                return new CreateUpdateResult { Option = CreateUpdateOptions.Ignore };
            else raceStartNumber.DriverId = driverId;
        if (horseId != "")
            if (raceStartNumber.HorseId == horseId)
                return new CreateUpdateResult { Option = CreateUpdateOptions.Ignore };
            else raceStartNumber.HorseId = horseId;
        
        
        // create race result item if not exists...
        var raceResultsExists = dataServices.RaceResultDataService.CheckExists(raceStartNumber.Id);
        if (!raceResultsExists) _raceResultsToCreate.Add(new RaceResult()
        {
            Id = Guid.NewGuid().ToString(),
            RaceStartNumberId = raceStartNumber.Id,
            Place = int.TryParse(resultData.Place, out var p) ? p : 0,
            Time = resultData.Time,
            Odds = int.TryParse(resultData.Odds, out var od) ? od : 0,
            KmTime = resultData.KmTime,
            RRemark = resultData.RRemark,
            GRemark = resultData.GRemark,
            FromDirectSource = false,
        });
        
        // return option for create or update
        return new CreateUpdateResult()
        {
            StartNumberData = raceStartNumber,
            Option = raceStartNumberExists ? CreateUpdateOptions.Update : CreateUpdateOptions.Create
        };
    }
    
    
    /// <summary>
    /// Handle lists for create and update. Will clear create and update lists
    /// </summary>
    private async Task StoreRaceData()
    {
        var startNumberChunks = _startNumbersToCreate.Chunk(500).ToList();
        var raceResultChunks = _raceResultsToCreate.Chunk(500).ToList();
        var startNumberDriverUpdateChunks = _startNumbersUpdateDriver.Chunk(500).ToList();
        var startNumberHorseUpdateChunks = _startNumbersUpdateHorse.Chunk(500).ToList();

        var count = startNumberChunks.Count
                    + raceResultChunks.Count
                    + startNumberDriverUpdateChunks.Count
                    + startNumberHorseUpdateChunks.Count;
        var message = "Create and update collected data";
        var options = CreateProgressBarOptions();
        using var bar = new ProgressBar(count, message, options);

        foreach (var chunk in startNumberChunks)
        {
            await dataServices.RaceStartNumberDataService.AddBulkAsync(chunk.ToList());
            bar.Tick();
        }

        foreach (var chunk in raceResultChunks)
        {
            await dataServices.RaceResultDataService.AddBulkAsync(chunk.ToList());
            bar.Tick();
        }

        foreach (var chunk in startNumberDriverUpdateChunks)
        {
            await dataServices.RaceStartNumberDataService.UpdateDriversBulkAsync(chunk.ToList());
            bar.Tick();
        }

        foreach (var chunk in startNumberHorseUpdateChunks)
        {
            await dataServices.RaceStartNumberDataService.UpdateHorsesBulkAsync(chunk.ToList());
            bar.Tick();
        }
        
        _startNumbersToCreate.Clear();
        _raceResultsToCreate.Clear();
        _startNumbersUpdateDriver.Clear();
        _startNumbersUpdateHorse.Clear();
    }
    /// <summary>
    /// Iterate collected result data and updates both driver and horse buffers;
    /// Will clear result data
    /// </summary>
    private void ResolveExtractData()
    {
        var driverTreshhold = 0;
        var horseTreshhold = 0;

        foreach (var d in _driverExtract)
            driverBuffer.Remove(d);
        foreach (var h in _horseExtract)
            horseBuffer.Remove(h);
        
        _driverExtract.Clear();
        _horseExtract.Clear();

        foreach (var d in driverBuffer)
        {
            var exist = dataServices.DriverDataService.CheckExists(d);
            if (exist) continue;
            if (++driverTreshhold > DriverBatchSize) continue;
            _driverExtract.Add(d);
        }

        foreach (var h in horseBuffer)
        {
            var exist = dataServices.HorseDataService.CheckExists(h);
            if (exist) continue;
            if (++horseTreshhold > HorseBatchSize) continue;
            _horseExtract.Add(h);
        }
        _driverResultsScrapeData.Clear();
        _horseResultsScrapeData.Clear();
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
            ForegroundColorDone = ConsoleColor.DarkCyan,
        };
    }
}