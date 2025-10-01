using Application.DataServices;
using Application.DataServices.Interfaces;
using Application.Pipelines.NO.Collection.UpdateStartlistAndResultStep;
using Models.DbModels.Updates;
using Models.ScrapeData;
using ShellProgressBar;

namespace Application.Pipelines.NO.Steps;

public class UpdateStartlistAndResultsStep(
    IDataServiceCollection dataServiceCollection,
    List<StartlistScrapeData> startlistScrapeData,
    List<ResultScrapeData> resultScrapeData)
{

    private UpdateStartlistAndResultReport Report = new UpdateStartlistAndResultReport();
    private readonly IDataServiceCollection _dataServiceCollection = dataServiceCollection;

    public async Task RunAsync()
    {
        
        if ((startlistScrapeData.Count + resultScrapeData.Count)  == 0) return;
        
        var count = startlistScrapeData.Count +  resultScrapeData.Count;
        var message = "Updating startlists and results list";
        var options = CreateProgressBarOptions();
        using var bar = new ProgressBar(count,  message, options);
        
        foreach (var item in startlistScrapeData)
        {
            await ParseStartlistData(item);
            bar.Tick();
        }

        foreach (var item in resultScrapeData)
        {
            await ParseResultsData(item);
            bar.Tick();
        }
        
        Report.Report();
    }

    private async Task ParseStartlistData(StartlistScrapeData rawData)
    {
        var racecourseNorm = rawData.RaceCourse.ToUpper();
        var racecourseExists = _dataServiceCollection.RaceCourseDataService.CheckExists(racecourseNorm);
        if (!racecourseExists)
        {
            Report.StartlistNoRacecourse++;
            return;
        }
        var racecourseId = _dataServiceCollection.RaceCourseDataService.GetModel(racecourseNorm).Id;

        var competitionKey = $"{racecourseId}_{rawData.Date}";
        var competitionExists = _dataServiceCollection.CompetitionDataService.CheckExists(competitionKey);
        if (!competitionExists)
        {
            Report.StartlistNoCompetition++;
            return;
        }
        var competitionId = _dataServiceCollection.CompetitionDataService.GetModel(competitionKey).Id;
        
        var raceNumber = int.Parse(rawData.RaceNumber.Split("-")[1]);
        var raceKey = $"{competitionId}_{raceNumber}";
        var raceExists = _dataServiceCollection.RaceDataService.CheckExists(raceKey);
        if (!raceExists)
        {
            Report.StartlistNoRace++;
            return;
        }
        var raceId = _dataServiceCollection.RaceDataService.GetModel(raceKey).Id;

        var startnumberKey = $"{raceId}_{rawData.StartNumber}";
        var startnumberExists = _dataServiceCollection.RaceStartNumberDataService.CheckExists(startnumberKey);
        if (!startnumberExists)
        {
            Report.StartlistNoStartnumber++;
            return;
        }
        var startnumberId = _dataServiceCollection.RaceStartNumberDataService.GetModel(startnumberKey).Id;
        
        var entity = _dataServiceCollection.RaceStartNumberDataService.GetModel(startnumberKey);
        if (entity.FromDirectSource) return;
            

        var updateItem = new RaceStartNumberUpdate()
        {
            Id = startnumberId,
            Auto = rawData.Auto,
            Turn = rawData.Turn,
            HasGambling = rawData.HasGambling,
        };
        
        await _dataServiceCollection.RaceStartNumberDataService.UpdateAsync(updateItem);
        Report.StartlistUpdated++;
    }

    private async Task ParseResultsData(ResultScrapeData rawData)
    {
        var racecourseNorm = rawData.RaceCourse.ToUpper();
        var racecourseExists = _dataServiceCollection.RaceCourseDataService.CheckExists(racecourseNorm);
        if (!racecourseExists)
        {
            Report.ResultNoRacecourse++;
            return;
        }
        var racecourseId = _dataServiceCollection.RaceCourseDataService.GetModel(racecourseNorm).Id;

        var competitionKey = $"{racecourseId}_{rawData.Date}";
        var competitionExists = _dataServiceCollection.CompetitionDataService.CheckExists(competitionKey);
        if (!competitionExists)
        {
            Report.ResultNoCompetition++;
            return;
        }
        var competitionId = _dataServiceCollection.CompetitionDataService.GetModel(competitionKey).Id;
        
        var raceNumber = int.Parse(rawData.RaceNumber.Split("-")[1]);
        var raceKey = $"{competitionId}_{raceNumber}";
        var raceExists = _dataServiceCollection.RaceDataService.CheckExists(raceKey);
        if (!raceExists)
        {
            Report.ResultNoRace++;
            return;
        }
        var raceId = _dataServiceCollection.RaceDataService.GetModel(raceKey).Id;

        var startnumberKey = $"{raceId}_{rawData.StartNumber}";
        var startnumberExists = _dataServiceCollection.RaceStartNumberDataService.CheckExists(startnumberKey);
        if (!startnumberExists)
        {
            Report.ResultNoStartnumber++;
            return;
        }
        var startnumberId = _dataServiceCollection.RaceStartNumberDataService.GetModel(startnumberKey).Id;
        
        var raceResultExist = _dataServiceCollection.RaceResultDataService.CheckExists(startnumberId);
        if (!raceResultExist)
        {
            Report.ResultNoResults++;
            return;
        }
        var raceResultId = _dataServiceCollection.RaceResultDataService.GetModel(startnumberId).Id;
        var entity = _dataServiceCollection.RaceResultDataService.GetModel(startnumberId);
        if (entity.FromDirectSource) return;

        var updateItem = new RaceResultUpdate()
        {
            Id = raceResultId,
            Time = rawData.Time,
        };
        
        await _dataServiceCollection.RaceResultDataService.UpdateAsync(updateItem);
        Report.ResultUpdated++;
    }
    
    private ProgressBarOptions CreateProgressBarOptions()
    {
        return new ProgressBarOptions
        {
            ForegroundColor = ConsoleColor.DarkCyan,
            BackgroundColor = ConsoleColor.White,
            ForegroundColorDone = ConsoleColor.DarkCyan,
        };
    }
}