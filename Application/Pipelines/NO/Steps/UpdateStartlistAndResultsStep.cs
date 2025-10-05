using Application.DataServices;
using Application.Pipelines.NO.Collection.UpdateStartlistAndResultStep;
using Models.DbModels.Updates;
using Models.ScrapeData;
using ShellProgressBar;

namespace Application.Pipelines.NO.Steps;

public class UpdateStartlistAndResultsStep(
    IDataServiceRegistry dataServiceRegistry,
    List<StartlistScrapeData> startlistScrapeData,
    List<ResultScrapeData> resultScrapeData)
{

    private UpdateStartlistAndResultReport _report = new();
    private IDataServiceRegistry _dataServiceRegistry = dataServiceRegistry;

    public async Task RunAsync()
    {
        
        if (startlistScrapeData.Count + resultScrapeData.Count  == 0) return;
        
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
        
        _report.Report();
    }

    private async Task ParseStartlistData(StartlistScrapeData rawData)
    {
        var racecourseNorm = rawData.RaceCourse.ToUpper();
        var racecourseExists = _dataServiceRegistry.RaceCourseDataService.CheckExists(racecourseNorm);
        if (!racecourseExists)
        {
            _report.StartlistNoRacecourse++;
            return;
        }
        var racecourseId = _dataServiceRegistry.RaceCourseDataService.GetModel(racecourseNorm).Id;

        var competitionKey = $"{racecourseId}_{rawData.Date}";
        var competitionExists = _dataServiceRegistry.CompetitionDataService.CheckExists(competitionKey);
        if (!competitionExists)
        {
            _report.StartlistNoCompetition++;
            return;
        }
        var competitionId = _dataServiceRegistry.CompetitionDataService.GetModel(competitionKey).Id;
        
        var raceNumber = int.Parse(rawData.RaceNumber.Split("-")[1]);
        var raceKey = $"{competitionId}_{raceNumber}";
        var raceExists = _dataServiceRegistry.RaceDataService.CheckExists(raceKey);
        if (!raceExists)
        {
            _report.StartlistNoRace++;
            return;
        }
        var raceId = _dataServiceRegistry.RaceDataService.GetModel(raceKey).Id;

        var startnumberKey = $"{raceId}_{rawData.StartNumber}";
        var startnumberExists = _dataServiceRegistry.RaceStartNumberDataService.CheckExists(startnumberKey);
        if (!startnumberExists)
        {
            _report.StartlistNoStartnumber++;
            return;
        }
        var startnumberId = _dataServiceRegistry.RaceStartNumberDataService.GetModel(startnumberKey).Id;
        
        var entity = _dataServiceRegistry.RaceStartNumberDataService.GetModel(startnumberKey);
        if (entity.FromDirectSource) return;
            

        var updateItem = new RaceStartNumberUpdate
        {
            Id = startnumberId,
            Auto = rawData.Auto,
            Turn = rawData.Turn,
            HasGambling = rawData.HasGambling
        };
        
        await _dataServiceRegistry.RaceStartNumberDataService.UpdateAsync(updateItem);
        _report.StartlistUpdated++;
    }

    private async Task ParseResultsData(ResultScrapeData rawData)
    {
        var racecourseNorm = rawData.RaceCourse.ToUpper();
        var racecourseExists = _dataServiceRegistry.RaceCourseDataService.CheckExists(racecourseNorm);
        if (!racecourseExists)
        {
            _report.ResultNoRacecourse++;
            return;
        }
        var racecourseId = _dataServiceRegistry.RaceCourseDataService.GetModel(racecourseNorm).Id;

        var competitionKey = $"{racecourseId}_{rawData.Date}";
        var competitionExists = _dataServiceRegistry.CompetitionDataService.CheckExists(competitionKey);
        if (!competitionExists)
        {
            _report.ResultNoCompetition++;
            return;
        }
        var competitionId = _dataServiceRegistry.CompetitionDataService.GetModel(competitionKey).Id;
        
        var raceNumber = int.Parse(rawData.RaceNumber.Split("-")[1]);
        var raceKey = $"{competitionId}_{raceNumber}";
        var raceExists = _dataServiceRegistry.RaceDataService.CheckExists(raceKey);
        if (!raceExists)
        {
            _report.ResultNoRace++;
            return;
        }
        var raceId = _dataServiceRegistry.RaceDataService.GetModel(raceKey).Id;

        var startnumberKey = $"{raceId}_{rawData.StartNumber}";
        var startnumberExists = _dataServiceRegistry.RaceStartNumberDataService.CheckExists(startnumberKey);
        if (!startnumberExists)
        {
            _report.ResultNoStartnumber++;
            return;
        }
        var startnumberId = _dataServiceRegistry.RaceStartNumberDataService.GetModel(startnumberKey).Id;
        
        var raceResultExist = _dataServiceRegistry.RaceResultDataService.CheckExists(startnumberId);
        if (!raceResultExist)
        {
            _report.ResultNoResults++;
            return;
        }
        var raceResultId = _dataServiceRegistry.RaceResultDataService.GetModel(startnumberId).Id;
        var entity = _dataServiceRegistry.RaceResultDataService.GetModel(startnumberId);
        if (entity.FromDirectSource) return;

        var updateItem = new RaceResultUpdate
        {
            Id = raceResultId,
            Time = rawData.Time
        };
        
        await _dataServiceRegistry.RaceResultDataService.UpdateAsync(updateItem);
        _report.ResultUpdated++;
    }
    
    private ProgressBarOptions CreateProgressBarOptions()
    {
        return new ProgressBarOptions
        {
            ForegroundColor = ConsoleColor.DarkCyan,
            BackgroundColor = ConsoleColor.White,
            ForegroundColorDone = ConsoleColor.DarkCyan
        };
    }
}