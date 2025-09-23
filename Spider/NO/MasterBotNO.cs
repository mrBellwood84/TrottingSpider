using Spider.NO.Options;

namespace Spider.NO;

public class MasterBotNo: BaseRobot
{
    // all years supported by website
    private readonly string[] _years = ["2010", "2011", "2012", "2013", "2014", "2015", "2016", "2017", "2018", "2019",
        "2020", "2021", "2022", "2023", "2024", "2025"];
    // all months supported by website
    private readonly string[] _months = ["1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12"];
    
    
    /// <summary>
    /// Run all years and months, collect data
    /// </summary>
    public async Task RunFull()
    {
        // itterate the _allCalendarDateMonthOptions
            // havest each calendar
            // parse calendar items
            // get all startlist data
            // harvest all drivers
                //parse them
            // harvest all horses
                // parse them
                    // itterate until buffer is clear
            // parse results
            
    }
    
    /// <summary>
    /// Run all months in provided year, collect data
    /// </summary>
    /// <param name="year"></param>
    public async Task RunYear(string year)
    {
        // run just single year
    }

    /// <summary>
    /// Run one month in provided year only.
    /// </summary>
    /// <param name="year"></param>
    /// <param name="month"></param>
    public async Task RunMonth(string year, string month)
    {
        // run single month in provided year
    }
    
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