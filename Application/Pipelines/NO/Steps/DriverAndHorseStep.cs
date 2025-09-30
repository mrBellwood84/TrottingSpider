using Application.DataServices.Interfaces;
using Models.DbModels;
using Models.DbModels.Updates;
using Models.ScrapeData;
using Models.Settings;
using Scraping.Processors;
using Scraping.Spider.NO;
using ShellProgressBar;

namespace Application.Pipelines.NO.Steps;

internal enum CreateUpdateOptions
{
    Create,
    Update
}

internal struct CreateUpdateResult
{
    public RaceStartNumber StartNumberData { get; init; }
    public CreateUpdateOptions Option { get; init; }
}

internal struct UpdateReport
{
    public UpdateReport() { }
    public int NewDrivers { get; set; } = 0;
    public int NewHorses { get; set; } = 0;
    public int NewDriverLicenses { get; set; } = 0;
    public int NewRacecourses { get; set; } = 0;
    public int NewCompetition { get; set; } = 0;
    public int NewRaces { get; set; } = 0;
    public int StartnumberCreated { get; set; } = 0;
    public int StartnumberUpdatedDriver { get; set; } = 0;
    public int StartnumberUpdatedHorses { get; set; } = 0;

    public void Report()
    {
        Console.WriteLine();
        if (NewDrivers > 0) AppLogger.LogPositive($"Drivers created: {NewDrivers}");
        if (NewHorses > 0) AppLogger.LogPositive($"Horses created: {NewHorses}");
        if (NewDriverLicenses > 0) AppLogger.LogPositive($"Driver licenses created: {NewDriverLicenses}");
        if (NewRacecourses > 0) AppLogger.LogPositive($"Races created: {NewRacecourses}");
        if (NewCompetition > 0) AppLogger.LogPositive($"Competition created: {NewCompetition}");
        if (NewRaces > 0) AppLogger.LogPositive($"Races created: {NewRaces}");
        if (StartnumberCreated > 0) AppLogger.LogPositive($"New start numbers created: {StartnumberCreated}");
        if (StartnumberUpdatedDriver > 0) AppLogger.LogPositive($"Drivers results updated: {StartnumberUpdatedDriver}");
        if (StartnumberUpdatedHorses > 0) AppLogger.LogPositive($"Horses results updated: {StartnumberUpdatedHorses}");
        
        NewDrivers = 0;
        NewHorses = 0;
        NewDriverLicenses = 0;
        NewRacecourses = 0;
        NewCompetition = 0;
        NewRaces = 0;
        StartnumberCreated = 0;
        StartnumberUpdatedDriver = 0;
        StartnumberUpdatedHorses = 0;
    }
}

public class DriverAndHorseStep(
    BrowserOptions browserOptions,
    ScraperSettings scraperSettings,
    IDataServiceCollection dataServices,
    HashSet<string> drivers,
    HashSet<string> horses)
{
    private readonly HashSet<string> _driverBuffer = [];
    private readonly HashSet<string> _horseBuffer = [];
    
    private readonly List<Driver> _newDrivers = [];
    private readonly List<DriverLicense> _newDriverLicenses = [];
    private readonly List<Horse> _newHorses = [];

    private readonly List<ResultScrapeData> _driverResultsScrapeData = [];
    private readonly List<ResultScrapeData> _horseResultsScrapeData = [];
    
    private readonly List<RaceStartNumber> _startNumbersToCreate = [];
    private readonly List<RaceStartNumber> _startNumbersUpdateDriver = [];
    private readonly List<RaceStartNumber> _startNumbersUpdateHorse = [];
    private readonly List<RaceResult> _raceResultsToCreate = [];
    
    private UpdateReport _updateReport = new UpdateReport();
    
    public async Task RunAsync()
    {
        AppLogger.LogDev("initializing driver and horses are limited!!!");
        int d_limit = 0;
        int h_limit = 0;
        int limit = 2;
            
        // init buffers, exclude existing drivers and horses
        foreach (var d in drivers)
        { 
            if (dataServices.DriverDataService.CheckExists(d)) continue; 
            if(++d_limit > limit) break;
            _driverBuffer.Add(d); 
        }
        foreach (var h in horses)
        {   
            if (dataServices.HorseDataService.CheckExists(h)) continue;
            if (++h_limit > limit) break;
            _horseBuffer.Add(h);
        }

        // clear driver and horse buffer here
        while (_driverBuffer.Count > 0 || _horseBuffer.Count > 0)
        {
            AppLogger.LogSubheader($"Resolving drivers : ({_driverBuffer.Count}) | horses: ({_horseBuffer.Count})");
            
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
            ResovleNewBuffers();
            
            _updateReport.Report();
        }
        
        // start loops here
        
        // remember to clear buffer!!!
    }

    /// <summary>
    /// Save collected driver and horse data
    /// </summary>
    private async Task SaveDriverHorseData()
    {
        if (_newDriverLicenses.Count > 0)
        {
            _updateReport.NewDriverLicenses++;
            await dataServices.DriverLicenseDataService.AddAsync(_newDriverLicenses);
        }
        
        _updateReport.NewDrivers += _newDrivers.Count;
        _updateReport.NewHorses += _newHorses.Count;

        var driverChunks = _newDrivers.Chunk(10).ToList();
        var horseChunks = _newHorses.Chunk(10).ToList();
        
        var count = driverChunks.Count + horseChunks.Count;
        var message = "Saving new drivers and hores to entity";
        var options = CreateProgressBarOptions();
        using var bar = new ProgressBar(count, message, options);
        
        foreach (var chunk in driverChunks)
        {
            await dataServices.DriverDataService.AddAsync(chunk.ToList());
            bar.Tick();
        }

        foreach (var chunk in horseChunks)
        {
            await dataServices.HorseDataService.AddAsync(chunk.ToList());
            bar.Tick();
        }
        
        _newDriverLicenses.Clear();
        _newDrivers.Clear();
        _newHorses.Clear();
    }


    /// <summary>
    /// Iterate driver buffer to gather data. Will clear buffer when complete
    /// </summary>
    private async Task ProcessDriverBuffer()
    {
        if (_driverBuffer.Count == 0) return;
        
        var message = "Collecting Driver data";
        var options = CreateProgressBarOptions();
        using var bar = new ProgressBar(_driverBuffer.Count, message, options);
        foreach (var d in _driverBuffer)
        {
            await CollectDriverData(d);
            bar.Tick();
        }
    }
    /// <summary>
    /// Collect driver data from browser!
    /// </summary>
    private async Task CollectDriverData(string driverSourceId)
    {
        var bot = new DriverBotNo(browserOptions, scraperSettings, driverSourceId);
        await bot.RunBrowser(bot.Execute);

        var dl = dataServices.DriverLicenseDataService.GetFullCache();
        var processor = new ProcessDriverScrapeData(dl);
        var newDriver = processor.Process(bot.DriverDataCollected);
        _newDrivers.Add(newDriver);
        _driverResultsScrapeData.AddRange(bot.RaceDataCollected);
        if (processor.NewDriverLicense != null)
            _newDriverLicenses.Add(processor.NewDriverLicense);
    }    
    /// <summary>
    /// Iterate horse buffer to gather data. Will clear buffer when complete.
    /// </summary>
    private async Task ProcessHorseBuffer()
    {
        if (_horseBuffer.Count == 0) return;
        
        var message = "Collecting Horse data";
        var options = CreateProgressBarOptions();
        using var bar = new ProgressBar(_horseBuffer.Count, message, options);
        foreach (var h in _horseBuffer)
        {
            await CollectHorseData(h);
            bar.Tick();
        }
    }
    /// <summary>
    /// Collect horse data from browser 
    /// </summary>
    private async Task CollectHorseData(string horseSourceId)
    {
        var bot = new HorseBotNo(browserOptions, scraperSettings, horseSourceId);
        await bot.RunBrowser(bot.Execute);

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
        
        var message = "Processing driver results";
        var options = CreateProgressBarOptions();
        using var bar = new ProgressBar(_driverResultsScrapeData.Count, message, options);
        foreach (var item in _driverResultsScrapeData)
        {
            var data = await ProcessResultData(item);
            var driverId = dataServices.DriverDataService.GetModel(item.DriverSourceId).Id;
            data.StartNumberData.DriverId = driverId;
            
            switch (data.Option)
            {
                case CreateUpdateOptions.Create:
                    _startNumbersToCreate.Add(data.StartNumberData);
                    _updateReport.StartnumberCreated++;
                    break;
                case CreateUpdateOptions.Update:
                    _startNumbersUpdateDriver.Add(data.StartNumberData);
                    _updateReport.StartnumberUpdatedDriver++;
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
        
        var message = "Processing horse results";
        var options = CreateProgressBarOptions();
        using var bar = new ProgressBar(_horseResultsScrapeData.Count, message, options);
        foreach (var item in _horseResultsScrapeData)
        {
            var data = await ProcessResultData(item);
            var horseId = dataServices.HorseDataService.GetModel(item.HorseSourceId).Id;
            data.StartNumberData.HorseId = horseId;
            
            switch (data.Option)
            {
                case CreateUpdateOptions.Create:
                    _startNumbersToCreate.Add(data.StartNumberData);
                    _updateReport.StartnumberCreated++;
                    break;
                case CreateUpdateOptions.Update:
                    _startNumbersUpdateHorse.Add(data.StartNumberData);
                    _updateReport.StartnumberUpdatedHorses++;
                    break;
            }
            bar.Tick();
        }
    }
    /// <summary>
    /// Process the collected data and resolve if create or update
    /// </summary>
    private async Task<CreateUpdateResult> ProcessResultData(ResultScrapeData resultData)
    {
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
            _updateReport.NewRacecourses++;
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
            _updateReport.NewCompetition++;
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
            _updateReport.NewRaces++;
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
        var startNumberChunks = _startNumbersToCreate.Chunk(100).ToList();
        var raceResultChunks = _raceResultsToCreate.Chunk(100).ToList();

        var count = startNumberChunks.Count
                    + raceResultChunks.Count
                    + _startNumbersUpdateDriver.Count
                    + _startNumbersUpdateHorse.Count;

        var message = "Create and update collected data";
        var options = CreateProgressBarOptions();
        
        using var bar = new ProgressBar(count, message, options);

        foreach (var chunk in startNumberChunks)
        {
            await dataServices.RaceStartNumberDataService.AddAsync(chunk.ToList());
            bar.Tick();
        }


        foreach (var chunk in raceResultChunks)
        {
            await dataServices.RaceResultDataService.AddAsync(chunk.ToList());
            bar.Tick();
        }

        foreach (var item in _startNumbersUpdateDriver)
        {
            await dataServices.RaceStartNumberDataService.UpdateDriverAsync(new RaceStartNumberUpdateDriver
            {
                Id = item.Id,
                DriverId = item.DriverId,
            });
            bar.Tick();
        }

        foreach (var item in _startNumbersUpdateHorse)
        {
            await dataServices.RaceStartNumberDataService.UpdateHorseAsync(new RaceStartNumberUpdateHorse
            {
                Id = item.Id,
                HorseId = item.HorseId,
            });
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
    private void ResovleNewBuffers()
    {
        _driverBuffer.Clear();  
        _horseBuffer.Clear();
        
        AppLogger.LogDev("Cutof added to resolve new horse and driver buffers");
        int hb_cutof = 0;
        int dr_cutof = 0;
        int limit = 2;
        foreach (var item in _driverResultsScrapeData)
        {
            if (++hb_cutof > limit) break;
            var exists = dataServices.HorseDataService.CheckExists(item.HorseSourceId);
            if (!exists) _horseBuffer.Add(item.HorseSourceId);
        }

        foreach (var item in _horseResultsScrapeData)
        {
            if (++dr_cutof > limit) break;
            var exists = dataServices.DriverDataService.CheckExists(item.DriverSourceId);
            if (!exists) _driverBuffer.Add(item.DriverSourceId);
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
            ProgressBarOnBottom = true,
            ForegroundColorDone = ConsoleColor.DarkCyan,
        };
    }
}