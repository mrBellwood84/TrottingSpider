using Application.DataServices.Interfaces;
using Models.DbModels;
using Models.ScrapeData;
using Models.Settings;
using Scraping.Processors;
using Scraping.Spider.NO;
using Scraping.Spider.NO.Options;

namespace Application.Pipelines.NO;

public class TestCalendarCollector
{
    private readonly BrowserOptions _browserOptions;
    private readonly ICompetitionDataService _competitionDataService;
    private readonly IRaceCourseDataService _raceCourseDataService;
    
    
    public TestCalendarCollector(
        BrowserOptions browserOptions, 
        ScraperSettings scraperSettings, 
        IRaceCourseDataService raceCourseDataService,
        ICompetitionDataService competitionDataService)
    {
       
        _browserOptions = browserOptions;
        _competitionDataService = competitionDataService;
        _raceCourseDataService = raceCourseDataService;
        
    }
    
    public async Task Run()
    {
        Console.Clear();
        Console.Title = "Testing Competition Collector Bot";
        Console.WriteLine("Testing Competition Collector!");
        
        await _competitionDataService.InitCache();
        await _raceCourseDataService.InitCache();
        
        var rawDataResult = new List<CalendarScrapeData>();
        var processedDataResult = new List<CalendarLinks>();
        
        // run bot for the range of year and months defined for test
        foreach (var option in _calendarYearMonthOptions())
        {
            var bot = new CalendarCollectorBot(_browserOptions, option.Year, option.Month);
            await bot.RunBrowser(bot.Run);
            rawDataResult.AddRange(bot.DataCollected);
        }
        
        // log result for tests
        Console.WriteLine("Calendar collector bot test complete");
        Console.WriteLine($"Calendar items collected: {rawDataResult.Count}");
        if (rawDataResult.Count > 0)
        {
            var item = rawDataResult[0];
            Console.WriteLine("\nData from first item of collected data");
            Console.WriteLine($"Date: {item.Date}");
            Console.WriteLine($"TrackTime: {item.CourseAndTime}");
            Console.WriteLine($"StartlistHref: {item.StartlistHref}");
            Console.WriteLine($"ResultHref: {item.ResultHref}\n");
        }
        
        // test the first 
        Console.WriteLine("\n\nTesting Calendar Data Processor, 5 items:");
        var processor = new CalendarDataProcessor();
        
        
        foreach (var item in rawDataResult)
        {
            var processed =  processor.Process(item);
            processedDataResult.Add(processed);
            var existInDb = _raceCourseDataService.CheckRaceCourseExists(processed.RaceCourseName);
            if (!existInDb)
            {
                Console.WriteLine($" => RaceCourse {processed.RaceCourseName} not found in database");
                Console.WriteLine($"    => New item will be added to database!!!");
                var newRaceCourse = new RaceCourse
                {
                    Id = processed.RaceCourseName,
                    Name = processed.RaceCourseName,
                };
                await _raceCourseDataService.AddNewRaceCourse(newRaceCourse);
            }
            var raceCourse = _raceCourseDataService.GetRaceCourse(processed.RaceCourseName);
            var competitionExists = _competitionDataService.CheckCompetitionExists($"{raceCourse.Id}_{processed.Date}");
            Console.WriteLine(competitionExists
                ? $"Competition {raceCourse.Name} @ {processed.Date} exists!"
                : $"Competition {raceCourse.Name} @ {processed.Date} does not exist!");
        }
    }

    private IEnumerable<CalendarDateMonthOptions> _calendarYearMonthOptions()
    {
        string[] testYears = ["2024", "2025"];
        string[] testMonths = ["1", "2"]; 
        for (int i = 0; i < testYears.Length; i++)
        for (int j = 0; j < testMonths.Length; j++)
        {
            var option = new CalendarDateMonthOptions
            {
                Year = testYears[i],
                Month = testMonths[j],
            };
            yield return option;
        }
                
    }
}