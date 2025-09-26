using Application.DataServices.Interfaces;
using Application.Pipelines.NO.Steps;
using Models.Settings;
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
    
    public async Task RunAsync()
    {
        // init data cache here
        await _dataServices.InitializeCache();
        
        foreach (var option in _calendarYearMonthOptions())
        { 
            
            // calendar step
            var calendarStep = new CalendarLinksCollectionStep(_browserOptions,_dataServices,option);
            var links = await calendarStep.RunAsync();
            
            // get and parse calendar options here
            // new tracks added to db

            // startlist step here 
            // get and parse startlist here
            // set buffer for horses and drivers

            // results step
            // parse results scraped data
            // check / add to horse and driver buffer

            // resolve driver and horse buffer
            // includes adding horses and drivers to db
            // add unknown racecourses to db
            // add competition / race / startnumbers / results to db

            // update step 
            // update startlists from parsed data!
            // update results from parsed data !
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