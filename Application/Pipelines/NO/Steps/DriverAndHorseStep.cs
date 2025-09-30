using System.Data;
using Application.DataServices.Interfaces;
using Models.DbModels;
using Models.DbModels.Updates;
using Models.ScrapeData;
using Models.Settings;
using Org.BouncyCastle.Bcpg.Attr;
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

public class DriverAndHorseStep(
    BrowserOptions browserOptions,
    ScraperSettings scraperSettings,
    IDataServiceCollection dataServices,
    HashSet<string> drivers,
    HashSet<string> horses)
{
    private HashSet<string> _driverBuffer = [];
    private HashSet<string> _horseBuffer = [];
    
    private readonly List<Driver> _newDrivers = [];
    private readonly List<DriverLicense> _newDriverLicenses = [];
    private readonly List<Horse> _newHorses = [];

    private readonly List<ResultScrapeData> _driverResultsScrapeData = [];
    private readonly List<ResultScrapeData> _horseResultsScrapeData = [];
    
    private readonly List<RaceStartNumber> _startNumbersToCreate = [];
    private readonly List<RaceStartNumber> _startNumbersUpdateDriver = [];
    private readonly List<RaceStartNumber> _startNumbersUpdateHorse = [];
    private readonly List<RaceResult> _raceResultsToCreate = [];
    
    public async Task RunAsync()
    {
        // init buffers here
        foreach (var d in drivers)
            _driverBuffer.Add(d);
        foreach (var h in horses)
            _horseBuffer.Add(h);
        
        // clear driver and horse buffer here
        while (_driverBuffer.Count > 0 || _horseBuffer.Count > 0)
        {
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

            // find unregisterd drivers and horses to add 
            ResovleNewBuffers();
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
            AppLogger.LogPositive($"Storing {_newDriverLicenses.Count} new driver licenses");
            await dataServices.DriverLicenseDataService.AddAsync(_newDriverLicenses);
        }
        
        if (_newDrivers.Count > 0) AppLogger.LogPositive($"Storing {_newDrivers.Count} new drivers");
        if (_newHorses.Count > 0) AppLogger.LogPositive($"Storing {_newHorses.Count} new horses");

        var driverChunks = _newDrivers.Chunk(100);
        var horseChunks = _newHorses.Chunk(100);
        var tasks = new List<Task>();

        foreach (var chunk in driverChunks)
            tasks.Add(dataServices.DriverDataService.AddAsync(chunk.ToList()));
        foreach (var chunk in horseChunks)
            tasks.Add(dataServices.HorseDataService.AddAsync(chunk.ToList()));
        
        await Task.WhenAll(tasks);
        
        _newDriverLicenses.Clear();
        _newDrivers.Clear();
        _newHorses.Clear();
    }    
    
    
    /// <summary>
    /// Iterate driver buffer to gather data. Will clear buffer when complete
    /// </summary>
    private async Task ProcessDriverBuffer()
    {
        AppLogger.LogSubheader($"Processing driver buffer: {_driverBuffer.Count} drivers!");
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
        AppLogger.LogSubheader($"Processing horse buffer: {_horseBuffer.Count} horses!");
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
        AppLogger.LogNeutral("Processing and storing results for collected driver data");
        foreach (var item in _driverResultsScrapeData)
        {
            var data = await ProcessResultData(item);
            var driverId = dataServices.DriverDataService.GetModel(item.DriverSourceId).Id;
            data.StartNumberData.DriverId = driverId;
            if (data.Option ==  CreateUpdateOptions.Create ) _startNumbersToCreate.Add(data.StartNumberData);
            if (data.Option == CreateUpdateOptions.Update) _startNumbersUpdateDriver.Add(data.StartNumberData);
        }
    }
    /// <summary>
    /// Iterate collected horse data for processing. Adds to create ad update lists
    /// </summary>
    private async Task ProcessHorseResults()
    {
        AppLogger.LogNeutral("Processing and storing results for collected horse data");
        foreach (var item in _horseResultsScrapeData)
        {
            var data = await ProcessResultData(item);
            var horseId = dataServices.HorseDataService.GetModel(item.HorseSourceId).Id;
            data.StartNumberData.HorseId = horseId;
            if (data.Option == CreateUpdateOptions.Create ) _startNumbersToCreate.Add(data.StartNumberData);
            if (data.Option == CreateUpdateOptions.Update) _startNumbersUpdateHorse.Add(data.StartNumberData);
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
            await dataServices.RaceCourseDataService.AddAsync(new Racecourse()
            {
                Id = Guid.NewGuid().ToString(),
                Name = racecourseNorm,
            });
        var racecourseId = dataServices.RaceCourseDataService.GetModel(racecourseNorm).Id;
        
        
        // get or create competition data item
        var date = FormatDateString(resultData.Date);
        var competitionKey = $"{racecourseId}_{date}";
        var competitionExists = dataServices.CompetitionDataService.CheckExists(competitionKey);
        if (!competitionExists)
            await dataServices.CompetitionDataService.AddAsync(new Competition()
            {
                Id = Guid.NewGuid().ToString(),
                RaceCourseId = racecourseId,
                Date = date
            });
        var competitionId = dataServices.CompetitionDataService.GetModel(competitionKey).Id;
        
        
        // get or create race data item
        var raceNumber = int.Parse(resultData.RaceNumber);
        var raceKey = $"{competitionId}_{raceNumber}";
        var race = dataServices.RaceDataService.CheckExists(raceKey);
        if (!race)
            await dataServices.RaceDataService.AddAsync(new Race()
            {
                Id = Guid.NewGuid().ToString(),
                CompetitionId = competitionId,
                RaceNumber = raceNumber,
                Distance = int.TryParse(resultData.Distance, out var d1) ? d1 : -1,
            });
        var raceId = dataServices.RaceDataService.GetModel(raceKey).Id;

        
        // get or create race startnumber item
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
            Place = int.TryParse(resultData.Place, out var p) ? p : -1,
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
        var startNumberChunks = _startNumbersToCreate.Chunk(100);
        var raceResultChunks = _raceResultsToCreate.Chunk(100);
        
        foreach (var chunk in startNumberChunks)
            await dataServices.RaceStartNumberDataService.AddAsync(chunk.ToList());
        
        foreach (var chunk in raceResultChunks)
            await dataServices.RaceResultDataService.AddAsync(chunk.ToList());
        
        foreach (var item in _startNumbersUpdateDriver)
        {
            await dataServices.RaceStartNumberDataService.UpdateDriverAsync(new RaceStartNumberUpdateDriver
            {
                Id = item.Id,
                DriverId = item.DriverId,
            });
        }

        foreach (var item in _startNumbersUpdateHorse)
        {
            await dataServices.RaceStartNumberDataService.UpdateHorseAsync(new RaceStartNumberUpdateHorse
            {
                Id = item.Id,
                HorseId = item.HorseId,
            });
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
        foreach (var item in _driverResultsScrapeData)
        {
            if (++hb_cutof > 3) break;
            var exists = dataServices.HorseDataService.CheckExists(item.HorseSourceId);
            if (!exists) _horseBuffer.Add(item.HorseSourceId);
        }

        foreach (var item in _horseResultsScrapeData)
        {
            if (++dr_cutof > 3) break;
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