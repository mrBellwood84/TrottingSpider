using Scraping.Spider.NO.Options;

namespace Scraping.Spider.NO;

public class MasterBotNo: BaseRobot
{
    // all years supported by website
    private readonly string[] _years = ["2010", "2011", "2012", "2013", "2014", "2015", "2016", "2017", "2018", "2019",
        "2020", "2021", "2022", "2023", "2024", "2025"];
    // all months supported by website
    private readonly string[] _months = ["1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12"];
    
    
    /// <summary>
    /// Create enumerable calendar date and month options
    /// </summary>
    /// <returns></returns>
    private IEnumerable<CalendarDateMonthOptions> _allCalendarDateMonthOptions()
    {
        for (var i = 0; i < _years.Length; i++)
        for (var j = 0; j < _months.Length; j++)
        {
            var option = new CalendarDateMonthOptions
            {
                Year = _years[i],
                Month = _months[j],
            };
            yield return option;
        }
    }
}