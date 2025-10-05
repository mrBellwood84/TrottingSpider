using Application.DataServices;
using Application.Pipelines.NO.Steps;
using Models.Settings;
using Scraping.Spider.NO.Options;

namespace Application.Pipelines.NO;

public class Pipeline(
    BrowserOptions browserOptions,
    ScraperSettings scraperSettings,
    IDataServiceRegistry dataServices,
    IBufferDataService bufferDataService)
{
    public async Task RunAsync()
    {
        var count = 0;
        var iterations = _resolveYearRange().Length * scraperSettings.MonthRange.Length;
        
        // init data cache here
        await dataServices.InitCaches();
        await bufferDataService.InitBuffers();

        if (bufferDataService.DriverBuffer.Count > 0 || bufferDataService.HorseBuffer.Count > 0)
        {
            var dataCollector = new DriverAndHorseStep(browserOptions, scraperSettings, 
                dataServices, bufferDataService);
            await dataCollector.RunAsync();
        }
        
        foreach (var option in _calendarYearMonthOptions())
        { 
            AppLogger.AppLogger.LogHeader($"Resolving {++count}/{iterations} - {option.Year} - {option.Month}");
            
            // calendar step
            var calendarStep = new CalendarLinksCollectionStep(browserOptions, dataServices, option);
            var calendarLinks = await calendarStep.RunAsync();
            
            // collect startlists and results data
            var startlistResultStep = new StartlistResultsCollectionStep(browserOptions, calendarLinks, bufferDataService);
            await startlistResultStep.RunAsync();
            
            var driverAndHorsesStep = new DriverAndHorseStep(browserOptions, scraperSettings,
                dataServices, bufferDataService);
            await driverAndHorsesStep.RunAsync();
            
            var updateStep = new UpdateStartlistAndResultsStep(
                dataServices, 
                startlistResultStep.StartlistDataCollected, 
                startlistResultStep.ResultDataCollected);
            await updateStep.RunAsync();
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
                Month = m
            };
            yield return option;
        }
    }
    
}