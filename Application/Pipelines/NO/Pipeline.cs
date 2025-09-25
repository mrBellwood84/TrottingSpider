using Models.Settings;
using Scraping.Spider.NO.Options;

namespace Application.Pipelines.NO;

public class Pipeline
{
    private readonly ScraperSettings _scraperSettings;
    private readonly string[] _limitedYearRange;

    public Pipeline(ScraperSettings scraperSettings)
    {
        _scraperSettings = scraperSettings;
        _limitedYearRange = _setLimitedYearRange(_scraperSettings.YearRange);
    }
    
    public async Task RunAsync()
    {
        // itterate year month as provided by scraper options
        // collect starlists and results link for provided year and month
        // open and collect data from startlist
        // open and collect data from results list
        // check sourceid for driver and horse. Unknown to buffer to be resolved
            // while buffers not cleared
                // get driver data
                // get horse data
                // add drivers and horses to database
                    // update caches and clear buffers
                    // unknows added to new buffer
        // parse startlists data
        // parse results data
    }
    
    private string[] _setLimitedYearRange(string[] yearRange)
    {
        var result = new List<string>();
        foreach (var year in yearRange)
        {
            if (int.Parse(year) <= int.Parse(_scraperSettings.MaxYear)
                && int.Parse(year) >= int.Parse(_scraperSettings.MinYear)) result.Add(year);
        }
        return result.ToArray();
    }
    private IEnumerable<CalendarDateMonthOptions> _allCalendarDateMonthOptions()
    {
        for (int i = 0; i < _limitedYearRange.Length; i++)
        for (int j = 0; j < _scraperSettings.MonthRange[0..2].Length; j++)
        {
            var option = new CalendarDateMonthOptions
            {
                Year = _limitedYearRange[i],
                Month = _scraperSettings.MonthRange[j],
            };
            yield return option;
        }
                
    }
}