using Application.DataServices.Interfaces;
using Application.Pipelines.NO.Steps;
using K4os.Compression.LZ4.Streams.Frames;
using Models.Settings;
using Scraping.Spider.NO;
using Scraping.Spider.NO.Options;

namespace Application.Pipelines.NO;

public class Pipeline
{
    // data services
    private readonly IDataServiceCollection _dataServices;
    
    // settings
    private readonly BrowserOptions _browserOptions;
    private readonly ScraperSettings _scraperSettings;

    public Pipeline(
        
        BrowserOptions browserOptions,
        ScraperSettings scraperSettings,
        IDataServiceCollection dataServices)
    {
        _browserOptions = browserOptions;
        _dataServices = dataServices;
        _scraperSettings = scraperSettings;
    }

    public async Task Test()
    {
        Console.WriteLine(" -- Testing startlists and results link --");
        
        var linklist = new List<CalendarLinks>();
        linklist.Add(new CalendarLinks
        {
            StartlistLink = "https://www.travsport.no/travbaner/jarlsberg-travbane/startlist/2025-09-21",
            ResultsLink = "https://www.travsport.no/travbaner/jarlsberg-travbane/results/2025-09-21",
        });
        linklist.Add(new CalendarLinks
        {
            StartlistLink = "https://www.travsport.no/travbaner/bergen-travpark/startlist/2025-09-25",
            ResultsLink = "https://www.travsport.no/travbaner/bergen-travpark/results/2025-09-25",
        });

        var startlistResultStep = new StartlistResultsCollectionStep(_browserOptions, linklist);
        await startlistResultStep.RunAsync();
        
        Console.WriteLine(" -- Testing complete ..");
        Console.WriteLine($"Startlist items collected: {startlistResultStep.StartlistDataCollected.Count}");
        Console.WriteLine($"Results collected: {startlistResultStep.ResultDataCollected.Count}");

        var startlistDrivers = new List<string>();
        var startlistHorses = new List<string>();
        var startlistUniqueDrivers = new HashSet<string>();
        var startlistUniqueHorses = new HashSet<string>();
        
        var resultListDrivers = new List<string>();
        var resultListHorses = new List<string>();
        var resultListUniqueDrivers = new HashSet<string>();
        var resultListUniqueHorses = new HashSet<string>();
        
        var allUniqueDrivers = new HashSet<string>();
        var allUniqueHorses = new HashSet<string>();
        

        foreach (var item in startlistResultStep.StartlistDataCollected)
        {
            startlistDrivers.Add(item.DriverSourceId);
            startlistHorses.Add(item.HorseSourceId);
            startlistUniqueDrivers.Add(item.DriverSourceId);
            startlistUniqueHorses.Add(item.HorseSourceId);
            
            allUniqueDrivers.Add(item.DriverSourceId);
            allUniqueHorses.Add(item.HorseSourceId);
        }

        foreach (var item in startlistResultStep.ResultDataCollected)
        {
            resultListDrivers.Add(item.DriverSourceId);
            resultListHorses.Add(item.HorseSourceId);
            resultListUniqueDrivers.Add(item.DriverSourceId);
            resultListUniqueHorses.Add(item.HorseSourceId);
            
            allUniqueDrivers.Add(item.DriverSourceId);
            allUniqueHorses.Add(item.HorseSourceId);
            
        }
        
        Console.WriteLine($"\nStartlist All drivers : {startlistDrivers.Count}");
        Console.WriteLine($"Startlist All horses : {startlistHorses.Count}");
        Console.WriteLine($"Startlist Unique drivers : {startlistUniqueDrivers.Count}");
        Console.WriteLine($"Startlist Unique horses : {startlistUniqueHorses.Count}");
        
        Console.WriteLine($"\nResultList drivers : {resultListDrivers.Count}");
        Console.WriteLine($"ResultList horses : {resultListHorses.Count}");
        Console.WriteLine($"Resultlist unique drivers : {resultListUniqueDrivers.Count}");
        Console.WriteLine($"Resultlist unique horses : {resultListUniqueHorses.Count}");
        
        Console.WriteLine($"\nAll unique drivers : {allUniqueDrivers.Count}");
        Console.WriteLine($"All unique horses : {allUniqueHorses.Count}");

        var startlistItem = startlistResultStep.StartlistDataCollected[0];
        Console.WriteLine("\n\n -- Startlist scrape data example");
        Console.WriteLine($"RaceCourse: {startlistItem.RaceCourse}");
        Console.WriteLine($"Date: {startlistItem.Date}");
        Console.WriteLine($"RaceNumber: {startlistItem.RaceNumber}");
        Console.WriteLine($"StartNumber: {startlistItem.StartNumber}");
        Console.WriteLine($"DriverSourceId: {startlistItem.DriverSourceId}");
        Console.WriteLine($"HorseSourceId: {startlistItem.HorseSourceId}");
        Console.WriteLine($"TrackNumber: {startlistItem.TrackNumber}");
        Console.WriteLine($"ForeShoe: {startlistItem.ForeShoe}");
        Console.WriteLine($"HindShoe: {startlistItem.HindShoe}");
        Console.WriteLine($"Turn: {startlistItem.Turn}");
        Console.WriteLine($"Auto: {startlistItem.Auto}");
        Console.WriteLine($"Distance: {startlistItem.Distance}");
        Console.WriteLine($"Cart: {startlistItem.Cart}");
        Console.WriteLine($"HasGambling: {startlistItem.HasGambling}");

        var resultItem = startlistResultStep.ResultDataCollected[0];
        Console.WriteLine("\n\n -- Results scrape data example");
        Console.WriteLine($"RaceCourse: {resultItem.RaceCourse}");
        Console.WriteLine($"Date: {resultItem.Date}");
        Console.WriteLine($"RaceNumber: {resultItem.RaceNumber}");
        Console.WriteLine($"StartNumber: {resultItem.StartNumber}");
        Console.WriteLine($"DriverSourceId: {resultItem.DriverSourceId}");
        Console.WriteLine($"HorseSourceId: {resultItem.HorseSourceId}");
        Console.WriteLine($"TrackNumber: {resultItem.TrackNumber}");
        Console.WriteLine($"Odds: {resultItem.Odds}");
        Console.WriteLine($"Distance: {resultItem.Distance}");
        Console.WriteLine($"ForeShoe: {resultItem.ForeShoe}");
        Console.WriteLine($"HindShoe: {resultItem.HindShoe}");
        Console.WriteLine($"Cart: {resultItem.Cart}");
        Console.WriteLine($"Place: {resultItem.Place}");
        Console.WriteLine($"Time: {resultItem.Time}");
        Console.WriteLine($"KmTime: {resultItem.KmTime}");
        Console.WriteLine($"RRemark: {resultItem.RRemark}");
        Console.WriteLine($"GRemark: {resultItem.GRemark}");
        Console.WriteLine($"FromDirectSource: {resultItem.FromDirectSource}");
        
        Console.WriteLine($"\nCollected in step class, Driver: {startlistResultStep.Drivers.Count}");
        Console.WriteLine($"Collected in step class, Horse: {startlistResultStep.Horses.Count}");
    }
    
    public async Task RunAsync()
    {
        // init data cache here
        await _dataServices.InitializeCache();
        
        foreach (var option in _calendarYearMonthOptions())
        { 
            // calendar step
            var calendarStep = new CalendarLinksCollectionStep(_browserOptions, _dataServices, option);
            var calendarLinks = await calendarStep.RunAsync();

            var startlistResultStep = new StartlistResultsCollectionStep(_browserOptions, calendarLinks);
            await startlistResultStep.RunAsync();
            
            Console.WriteLine($"DEV :: Drivers to collect: {startlistResultStep.Drivers.Count}");
            Console.WriteLine($"DEV :: Horses to collect: {startlistResultStep.Horses.Count}");
            
            break;

            // resolve drivers and horses. 
            // keep updating driver and horse buffer untill they are both depleted
            // this step will also add competitions / races /startnumbers and results to db and buffers

            // update competition / race / startnumber / results from direct source, see above! 
        }
    }
    
    /// <summary>
    /// Resolve year range based on year array and min/max years set in ScraperSettings.
    /// These settings are defined in "appsettings.json"
    /// </summary>
    private string[] _resolveYearRange(string[] years)
    {
        var result = new List<string>();
        foreach (var year in years)
        {
            var parsedYear = int.Parse(year);
            if (parsedYear > int.Parse(_scraperSettings.MaxYear)) continue;
            if (parsedYear < int.Parse(_scraperSettings.MinYear)) continue;
            result.Add(year);
        }
        return result.ToArray();
    }
    
    /// <summary>
    /// Produces year and month for the calendar link collector within limits of scraper settings
    /// </summary>
    private IEnumerable<CalendarDateMonthOptions> _calendarYearMonthOptions()
    {
        var years = _resolveYearRange(_scraperSettings.YearRange);
        foreach (var y in years)
        foreach (var m in _scraperSettings.MonthRange)
        {
            var option = new CalendarDateMonthOptions
            {
                Year = y,
                Month = m,
            };
            yield return option;
        }
    }
    
}