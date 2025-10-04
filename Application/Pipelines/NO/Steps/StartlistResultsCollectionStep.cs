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
    IDataServiceCollection dataServices,
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
        
        AppLogger.LogDev("Limiting startlist links");
        if (_calendarLinks.Count > 1) _calendarLinks = _calendarLinks[..1];
        
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
        

        var count = StartlistDataCollected.Count + ResultDataCollected.Count;
        var message2 = "Adding drivers and horses to buffer";
        using var bar2 = new ProgressBar(count, message2, options);
        
        foreach (var item in StartlistDataCollected)
        {
            await ResolveDriverBuffer(item.DriverSourceId);
            await ResolveHorseBuffer(item.HorseSourceId);
            bar2.Tick();
        }

        foreach (var item in ResultDataCollected)
        {
            await ResolveDriverBuffer(item.DriverSourceId);
            await ResolveHorseBuffer(item.HorseSourceId);
            bar2.Tick();
        }
    }

    private async Task ResolveDriverBuffer(string driverSourceId)
    {
        var driverResolved = dataServices.DriverDataService.CheckExists(driverSourceId);
        if (driverResolved) return;
        var driverInBuffer = bufferDataService.DriverBuffer.Contains(driverSourceId);
        if (driverInBuffer) return;
        await bufferDataService.AddDriverAsync(driverSourceId);
    }

    private async Task ResolveHorseBuffer(string horseSourceId)
    {
        var horseResolved = dataServices.HorseDataService.CheckExists(horseSourceId);
        if (horseResolved) return;
        var horseInBuffer = bufferDataService.HorseBuffer.Contains(horseSourceId);
        if (horseInBuffer) return;
        await bufferDataService.AddHorseAsync(horseSourceId);
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