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
        AppLogger.LogSubheader("Collecting links");
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
            var raceCourseExists = dataServices.RaceCourseDataService
                .CheckExists(processed.RaceCourseName);

            if (!raceCourseExists)
            {
                // add racecourse to database if not exist
                await dataServices.RaceCourseDataService.AddAsync(new Racecourse
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = processed.RaceCourseName,
                });
                AppLogger.LogPositive($"New race course added: {processed.RaceCourseName}");
            }
            
            // get racecourse item and create a valid competition dict key
            var raceCourseDbItem = dataServices.RaceCourseDataService.GetModel(processed.RaceCourseName);
            var competitionKey = $"{raceCourseDbItem.Id}_{processed.Date}";
            
            // check for competition exists in database
            var competitionExists = dataServices.CompetitionDataService
                .CheckExists(competitionKey);

            if (competitionExists)
            {
                // ignore links if both have confirmed collected data from source!!!
                var entity =  dataServices.CompetitionDataService.GetModel(competitionKey);
                if (entity is { StartlistFromSource: true, ResultsFromSource: true }) continue;
                
                // update processed data to concur with from source data in db entity
                processed.StartlistFromSource = entity.StartlistFromSource;
                processed.ResultsFromSource = entity.ResultsFromSource;
            }
            _processedData.Add(processed);
        }
        
        // log and process data
        AppLogger.LogNeutral($"Start number and result links collected: {_processedData.Count}");
        return _processedData;
    }
}