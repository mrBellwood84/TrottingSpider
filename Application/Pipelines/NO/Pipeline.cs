using Application.DataServices.Interfaces;
using Application.Pipelines.NO.Steps;
using Models.DbModels;
using Models.ScrapeData;
using Models.Settings;
using Scraping.Spider.NO;
using Scraping.Spider.NO.Options;

namespace Application.Pipelines.NO;

public class Pipeline(
    BrowserOptions browserOptions,
    ScraperSettings scraperSettings,
    IDataServiceCollection dataServices)
{



    public async Task RunAsync()
    {
        var count = 0;
        var iterations = _resolveYearRange().Length * scraperSettings.MonthRange.Length;
        
        // init data cache here
        await dataServices.InitCaches();
        
        foreach (var option in _calendarYearMonthOptions())
        { 
            AppLogger.LogHeader($"Resolving {++count}/{iterations} - {option.Year} - {option.Month}");
            
            /*
            // calendar step
            var calendarStep = new CalendarLinksCollectionStep(browserOptions, dataServices, option);
            var calendarLinks = await calendarStep.RunAsync();
            
            // collect startlists and results data
            var startlistResultStep = new StartlistResultsCollectionStep(browserOptions, calendarLinks);
            await startlistResultStep.RunAsync();
            */

            HashSet<string> mockDrivers = ["30040437"];
            HashSet<string> mockHorses = ["578001020185185"];
            
            var driverAndHorsesStep = new DriverAndHorseStep(browserOptions, scraperSettings, dataServices,
                mockDrivers, mockHorses);
            await driverAndHorsesStep.RunAsync();

            // resolve drivers and horses. 
            // keep updating driver and horse buffer until they are both depleted
            // this step will also add competitions / races /start numbers and results to db and buffers

            // update competition / race / start number / results from direct source, see above! 
        }
    }
    
    /// <summary>
    /// Resolve year range based on year array and min/max years set in ScraperSettings.
    /// These settings are defined in "appsettings.json"
    /// </summary>
    private string[] _resolveYearRange()
    {
        var result = new List<string>();
        foreach (var year in scraperSettings.YearRange)
        {
            var parsedYear = int.Parse(year);
            if (parsedYear > int.Parse(scraperSettings.MaxYear)) continue;
            if (parsedYear < int.Parse(scraperSettings.MinYear)) continue;
            result.Add(year);
        }
        return result.ToArray();
    }
    
    /// <summary>
    /// Produces year and month for the calendar link collector within limits of scraper settings
    /// </summary>
    private IEnumerable<CalendarDateMonthOptions> _calendarYearMonthOptions()
    {
        var years = _resolveYearRange();
        foreach (var y in years)
        foreach (var m in scraperSettings.MonthRange)
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