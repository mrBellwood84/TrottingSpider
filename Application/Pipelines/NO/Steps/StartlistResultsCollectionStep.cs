using Application.AppLogger;
using Application.DataServices;
using Models.ScrapeData;
using Models.Settings;
using Scraping.Errors;
using Scraping.Spider.NO;
using Scraping.Spider.NO.Options;
using ShellProgressBar;

namespace Application.Pipelines.NO.Steps;

public class StartlistResultsCollectionStep(
    BrowserOptions browserOptions,
    List<CalendarLinks> calendarLinks,
    IBufferDataService bufferDataService)
{
    public List<StartlistScrapeData> StartlistDataCollected { get; } = [];
    public List<ResultScrapeData> ResultDataCollected { get; } = [];

    private readonly HashSet<string> _drivers = [];
    private readonly HashSet<string> _horses = [];

    public async Task RunAsync()
    {
        // skip this function if no links are collected
        if (calendarLinks.Count == 0) return;
        
        // resolve progressbar params
        var message = "Collecting startlists and results!";
        var options = CreateProgressBarOptions();
        
        using (var bar = new ProgressBar(calendarLinks.Count, message, options))
            foreach (var item in calendarLinks)
            {
                try
                {
                    await CollectData(item);
                    bar.Tick();
                }
                catch (NoContentException)
                {
                    await FileLogger.AddToNoContentError(item.StartlistLink);
                }
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
        
        await bufferDataService.AddDriverBulkAsync(_drivers.ToList());
        await bufferDataService.AddHorseBulkAsync(_horses.ToList());
    }

    private async Task CollectData(CalendarLinks links)
    {
        var tasks = new List<Task>();
        var startlistBot = new StartlistBotNo(browserOptions, links.StartlistLink);
        var resultBot = new ResultsBotNo(browserOptions, links.ResultsLink);
        
        if(!links.StartlistFromSource && (links.StartlistLink != "")) 
            tasks.Add(startlistBot.RunBrowser(startlistBot.Execute));
        if (!links.ResultsFromSource && (links.ResultsLink != ""))
            tasks.Add(resultBot.RunBrowser(resultBot.Execute));
        
        await Task.WhenAll(tasks);
        StartlistDataCollected.AddRange(startlistBot.CollectedData);
        ResultDataCollected.AddRange(resultBot.DataCollected);
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