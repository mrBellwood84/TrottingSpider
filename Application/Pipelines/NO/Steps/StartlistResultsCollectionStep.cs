using Application.DataServices.Interfaces;
using Models.ScrapeData;
using Models.Settings;
using Scraping.Spider.NO;
using Scraping.Spider.NO.Options;

namespace Application.Pipelines.NO.Steps;

public class StartlistResultsCollectionStep(
    BrowserOptions browserOptions,
    List<CalendarLinks> calendarLinks)
{
    public List<StartlistScrapeData> StartlistDataCollected = [];
    public List<ResultScrapeData> ResultDataCollected = [];
    
    public HashSet<string> Drivers = [];
    public HashSet<string> Horses = [];

    public async Task RunAsync()
    {
        foreach (var item in calendarLinks)
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
}