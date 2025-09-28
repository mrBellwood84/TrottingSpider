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

    private async Task ProcessDriverResultData()
    {
        
    }

    private async Task ProcessHorseResultData()
    {
        
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