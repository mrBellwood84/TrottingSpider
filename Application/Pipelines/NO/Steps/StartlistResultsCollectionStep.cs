using Application.DataServices.Interfaces;
using Models.ScrapeData;
using Models.Settings;
using Scraping.Spider.NO;
using Scraping.Spider.NO.Options;
using ShellProgressBar;

namespace Application.Pipelines.NO.Steps;

public class StartlistResultsCollectionStep(
    BrowserOptions browserOptions,
    List<CalendarLinks> calendarLinks,
    IBufferDataService bufferDataService)
{
    public readonly List<StartlistScrapeData> StartlistDataCollected = [];
    public readonly List<ResultScrapeData> ResultDataCollected = [];

    private readonly HashSet<string> _drivers = [];
    private readonly HashSet<string> _horses = [];
    private List<CalendarLinks> _calendarLinks = calendarLinks;

    public async Task RunAsync()
    {
        // skip this function if no links are collected
        if (_calendarLinks.Count == 0) return;
        
        // resolve progressbar params
        var message = "Collecting startlists and results!";
        var options = CreateProgressBarOptions();
        
        using (var bar = new ProgressBar(_calendarLinks.Count, message, options))
            foreach (var item in _calendarLinks)
            {
                var tasks = new List<Task>();
                var startlistBot = new StartlistBotNo(browserOptions, item.StartlistLink);
                var resultBot = new ResultsBotNo(browserOptions, item.ResultsLink);

                if (!item.StartlistFromSource)
                    tasks.Add(startlistBot.RunBrowser(startlistBot.Execute));

                if (!item.ResultsFromSource)
                    tasks.Add(resultBot.RunBrowser(resultBot.Execute));

                await Task.WhenAll(tasks);

                StartlistDataCollected.AddRange(startlistBot.CollectedData);
                ResultDataCollected.AddRange(resultBot.DataCollected);
                bar.Tick();
            }

        await SetDriverHorseBuffers();
    }

    
    private async Task SetDriverHorseBuffers()
    {
        foreach (var item in StartlistDataCollected)
        {
            _drivers.Add(item.DriverSourceId);
            _horses.Add(item.HorseSourceId);
        }

        foreach (var item in ResultDataCollected)
        {
            _drivers.Add(item.DriverSourceId);
            _horses.Add(item.HorseSourceId);
        }
        
        AppLogger.LogPositive("Setting data collection buffers");
        await bufferDataService.AddDriverBulkAsync(_drivers.ToList());
        await bufferDataService.AddHorseBulkAsync(_horses.ToList());
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