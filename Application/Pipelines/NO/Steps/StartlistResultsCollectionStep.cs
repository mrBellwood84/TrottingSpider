using Models.ScrapeData;
using Models.Settings;
using Scraping.Spider.NO;
using Scraping.Spider.NO.Options;
using ShellProgressBar;

namespace Application.Pipelines.NO.Steps;

public class StartlistResultsCollectionStep(
    BrowserOptions browserOptions,
    List<CalendarLinks> calendarLinks)
{
    public List<StartlistScrapeData> StartlistDataCollected = [];
    public List<ResultScrapeData> ResultDataCollected = [];
    
    public HashSet<string> Drivers = [];
    public HashSet<string> Horses = [];
    private List<CalendarLinks> _calendarLinks = calendarLinks;

    public async Task RunAsync()
    {
        // resolve progressbar params
        var message = "Collecting startlists and results!";
        var options = CreateProgressBarOptions();
        
        AppLogger.LogDev("Limiting calendar links!!!");
        _calendarLinks = _calendarLinks[0..2];
        
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

        foreach (var item in StartlistDataCollected)
        {
            Drivers.Add(item.DriverSourceId);
            Horses.Add(item.HorseSourceId);
        }

        foreach (var item in ResultDataCollected)
        {
            Drivers.Add(item.DriverSourceId);
            Horses.Add(item.HorseSourceId);
        }
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