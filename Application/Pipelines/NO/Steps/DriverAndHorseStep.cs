using System.Data;
using Application.DataServices.Interfaces;
using Models.DbModels;
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
    HashSet<string> drivers,
    HashSet<string> horses)
{
    private HashSet<string> _driverBuffer = new HashSet<string>();
    private HashSet<string> _horseBuffer = new HashSet<string>();
    
    private readonly List<Driver> _newDrivers = [];
    private readonly List<DriverLicense> _newDriverLicenses = [];
    private readonly List<Horse> _newHorses = [];

    private readonly List<ResultScrapeData> _driverResultsScrapeData = [];
    private readonly List<ResultScrapeData> _horseResultsScrapeData = [];
    
    private readonly List<RaceStartNumber> _raceStartNumbersToAdd = [];
    private readonly List<RaceResult> _raceResultsToAdd = [];
    
    public async Task RunAsync()
    {
        // init buffers here
        foreach (var d in drivers)
            _driverBuffer.Add(d);
        foreach (var h in horses)
            _horseBuffer.Add(h);
        
        // clear driver and horse buffer here
        while (_driverBuffer.Count > 0 && _horseBuffer.Count > 0)
        {
            FilterDriverBuffer();
            FilterHorseBuffer();
            
            await ProcessDriverBuffer();
            await ProcessHorseBuffer();

            await SaveDriverHorseData();

            await ProcessDriverResults();
            await ProcessHorseResults();
            
            // crear add and update arrays ond report added numbers


            // parse result data
        }
        
        // start loops here
        
        // remember to clear buffer!!!
    }

    private void FilterDriverBuffer()
    {
        var updated = new HashSet<string>();
        foreach (var d in _driverBuffer)
        {
            var exists = dataServices.DriverDataService.CheckExists(d);
            if (!exists) updated.Add(d);
        }
        _driverBuffer = updated;
    }

    private void FilterHorseBuffer()
    {
        var updated = new HashSet<string>();
        foreach (var h in _horseBuffer)
        {
            var exists = dataServices.HorseDataService.CheckExists(h);
            if (!exists) updated.Add(h);
        }
        _horseBuffer = updated;
    }
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
        _driverBuffer.Clear();
    }
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
        _horseBuffer.Clear();
    }
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
    private async Task CollectHorseData(string horseSourceId)
    {
        var bot = new HorseBotNo(browserOptions, scraperSettings, horseSourceId);
        await bot.RunBrowser(bot.Execute);

        var processor = new ProcessHorseScrapeData();
        var newHorse = processor.Process(bot.HorseDataCollected);
        _newHorses.Add(newHorse);
        _horseResultsScrapeData.AddRange(bot.RaceDataCollected);
    }

    private async Task ProcessDriverResults()
    {
        AppLogger.LogNeutral("Processing and storing results for collected driver data");
        foreach (var item in _driverResultsScrapeData)
        {
            var data = await ProcessResultData(item);
            var driverId = dataServices.DriverDataService.GetModel(item.DriverSourceId).Id;
            data.DriverId = driverId;
            _raceStartNumbersToAdd.Add(data);
        }

        var snChunks = _raceStartNumbersToAdd.Chunk(100);
        var resChunks = _raceResultsToAdd.Chunk(100);

        foreach (var chunk in snChunks)
            await dataServices.RaceStartNumberDataService.AddAsync(chunk.ToList());
        
        foreach (var chunk in resChunks) 
            await dataServices.RaceResultDataService.AddAsync(chunk.ToList());
        
        _raceStartNumbersToAdd.Clear();
        _raceResultsToAdd.Clear();
    }
    private async Task ProcessHorseResults()
    {
        AppLogger.LogNeutral("Processing and storing results for collected horse data");
        foreach (var item in _horseResultsScrapeData)
        {
            var data = await ProcessResultData(item);
            var horseId = dataServices.HorseDataService.GetModel(item.HorseSourceId).Id;
            data.HorseId = horseId;
            _raceStartNumbersToAdd.Add(data);
        }
        
        var snChunks = _raceStartNumbersToAdd.Chunk(100);
        var resChunks = _raceResultsToAdd.Chunk(100);
        foreach (var chunk in snChunks)
            
            await dataServices.RaceStartNumberDataService.AddAsync(chunk.ToList());
        foreach (var chunk in resChunks)
            await dataServices.RaceResultDataService.AddAsync(chunk.ToList());
    }
    private async Task<RaceStartNumber> ProcessResultData(ResultScrapeData resultData)
    {
        // check
        var racecourseNorm = resultData.RaceCourse.ToUpper();
        var racecourseExists = dataServices.RaceCourseDataService.CheckExists(racecourseNorm);
        
        if (!racecourseExists)
            await dataServices.RaceCourseDataService.AddAsync(new Racecourse()
            {
                Id = Guid.NewGuid().ToString(),
                Name = racecourseNorm,
            });
        var racecourseId = dataServices.RaceCourseDataService.GetModel(racecourseNorm).Id;
        
        var competitionExists = dataServices.CompetitionDataService.CheckExists($"{racecourseId}_{resultData.Date}");
        if (!competitionExists)
            await dataServices.CompetitionDataService.AddAsync(new Competition()
            {
                Id = Guid.NewGuid().ToString(),
                RaceCourseId = racecourseId,
                Date = resultData.Date,
            });
        var competitionId = dataServices.CompetitionDataService.GetModel($"{racecourseId}_{resultData.Date}").Id;
        
        var raceNumber = int.TryParse(resultData.RaceNumber, out var rn) ? rn : 0;
        var race = dataServices.RaceDataService.CheckExists($"{competitionId}_{resultData.RaceNumber}");
        if (!race)
            await dataServices.RaceDataService.AddAsync(new Race()
            {
                Id = Guid.NewGuid().ToString(),
                CompetitionId = competitionId,
                RaceNumber = raceNumber,
                Distance = int.TryParse(resultData.Distance, out var d1) ? d1 : -1,
            });
        var raceId = dataServices.RaceDataService.GetModel($"{competitionId}_{raceNumber}").Id;

        var raceStartNumberExists =
            dataServices.RaceStartNumberDataService.CheckExists($"{raceId}_{resultData.StartNumber}");
        var raceStartNumber = raceStartNumberExists
            ? dataServices.RaceStartNumberDataService.GetModel($"{raceId}_{resultData.StartNumber}")
            : new RaceStartNumber()
            {
                Id = Guid.NewGuid().ToString(),
                RaceId = raceId,
                ProgramNumber = int.TryParse(resultData.StartNumber, out var sn) ? sn : -1,
                TrackNumber = int.TryParse(resultData.TrackNumber, out var t) ? t : -1,
                Distance = int.TryParse(resultData.Distance, out var d2) ? d2 : -1,
                ForeShoe = ResolveForeShoe(resultData.ForeShoe),
                HindShoe = ResolveHindShoe(resultData.HindShoe),
                Cart = resultData.Cart,
                FromDirectSource = false,
            };
        
        var raceResultsExists = dataServices.RaceResultDataService.CheckExists(raceStartNumber.Id);
        var raceResult = raceResultsExists
            ? dataServices.RaceResultDataService.GetModel(raceStartNumber.Id)
            : new RaceResult()
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
            };
        
        _raceResultsToAdd.Add(raceResult);

        return raceStartNumber;
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