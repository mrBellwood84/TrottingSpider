using Application.DataServices.Interfaces;
using Models.DbModels;
using Models.Settings;
using Scraping.Processors;
using Scraping.Spider.NO;
using Scraping.Spider.NO.Options;

namespace Application.Pipelines.NO.Steps;

public class CalendarLinksCollectionStep(
    BrowserOptions browserOptions,
    IDataServiceCollection dataServices,
    CalendarDateMonthOptions calendarOptions
    )
{
    private readonly List<CalendarLinks> _processedData = [];

    public async Task<List<CalendarLinks>> RunAsync()
    {
        Console.WriteLine($"\n -- Collecting Links for Year {calendarOptions.Year} - Month {calendarOptions.Month} --");
        
        // create bot and processor
        var bot = new CalendarCollectorBot(browserOptions, calendarOptions.Year, calendarOptions.Month);
        var processor = new CalendarDataProcessor();
        
        // run bot
        await bot.RunBrowser(bot.Execute);
        
        // parse raw data
        foreach (var item in bot.DataCollected)
        {
            // process data
            var processed = processor.Process(item);
            
            // check if racecourse and create if not exists
            var raceCourseExists = dataServices.RaceCourse
                .CheckRaceCourseExists(processed.RaceCourseName);

            if (!raceCourseExists)
            {
                // add racecourse to database if not exist
                await dataServices.RaceCourse.AddNewRaceCourse(new RaceCourse
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = processed.RaceCourseName,
                });
            }
            
            // get racecourse item and create a valid competition dict key
            var raceCourseDbItem = dataServices.RaceCourse.GetRaceCourse(processed.RaceCourseName);
            var competitionKey = $"{raceCourseDbItem.Id}_{processed.Date}";
            
            // check for competition exists in databae
            var competitionExists = dataServices.Competition
                .CheckCompetitionExists(competitionKey);

            if (competitionExists)
            {
                // ignore links if both have confirmed collected data from source!!!
                var entity =  dataServices.Competition.GetCompetition(competitionKey);
                if (entity is { StartlistFromSource: true, ResultsFromSource: true }) continue;
                
                // update processed data to concur with from source data in db entity
                processed.StartlistFromSource = entity.StartlistFromSource;
                processed.ResultsFromSource = entity.ResultsFromSource;
            }
            _processedData.Add(processed);
        }
        
        // log and process data
        Console.WriteLine($" - Competitions collected .: {bot.DataCollected.Count}");
        Console.WriteLine($" - Approved for harvesting : {_processedData.Count}");
        return _processedData;
    }
}