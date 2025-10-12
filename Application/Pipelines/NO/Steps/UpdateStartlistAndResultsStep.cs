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

    private UpdateStartlistAndResultReport _report = new UpdateStartlistAndResultReport();
    
    public async Task RunAsync()
    {
        _report.Startlists = startlistScrapeData.Count;
        _report.Results = resultScrapeData.Count;

        if (startlistScrapeData.Count + resultScrapeData.Count == 0)
        {
            _report.PrintReport();
            return;
        }

        var count = startlistScrapeData.Count + resultScrapeData.Count;
        var message = "Updating startlists and results list";
        var options = CreateProgressBarOptions();
        using var bar = new ProgressBar(count, message, options);

        foreach (var item in startlistScrapeData)
        {
            await ParseStartlistData(item);
            bar.Tick();
        }

        foreach (var item in resultScrapeData)
        {
            await ParseResults(item);
            bar.Tick();
        }
        
    }

    private async Task ParseStartlistData(StartlistScrapeData data)
    {
        var racecourseNorm = data.RaceCourse.ToUpper();
        var racecourseExists = dataServiceRegistry.RaceCourseDataService.CheckExists(racecourseNorm);
        if (!racecourseExists)
        {
            _report.StartlistRacecourseNotFound.Add(racecourseNorm);
            return;
        }
        var racecourseId = dataServiceRegistry.RaceCourseDataService.GetModel(racecourseNorm).Id;

        var competitionKey = $"{racecourseId}_{data.Date}";
        var competitionExists = dataServiceRegistry.CompetitionDataService.CheckExists(competitionKey);
        if (!competitionExists)
        {
            var res = $"{racecourseNorm} - {data.Date}";
            _report.StartlistCompetitionNotFound.Add(res);
            return;
        }
        var competitionEntity = dataServiceRegistry.CompetitionDataService.GetModel(competitionKey);
        var competitionId = competitionEntity.Id;
        
        if (competitionEntity.StartlistFromSource) return;
        
        var raceNumber = int.Parse(data.RaceNumber.Split("-")[1]);
        var raceKey = $"{competitionId}_{raceNumber}";
        var raceExists = dataServiceRegistry.RaceDataService.CheckExists(raceKey);
        if (!raceExists)
        {
            var res = $"{racecourseNorm} - {data.Date} - {raceNumber}";
            _report.StartlistRaceNotFound.Add(res);
            return;
        }
        var raceId = dataServiceRegistry.RaceDataService.GetModel(raceKey).Id;
        
        var startnumberKey = $"{raceId}_{data.StartNumber}";
        var startnumberExists = dataServiceRegistry.RaceStartNumberDataService.CheckExists(startnumberKey);
        if (!startnumberExists)
        {
            var res = $"{racecourseNorm} - {data.Date} - {raceNumber} - {data.StartNumber}";
            _report.StartlistStartnumberNotFound.Add(res);
            return;
        }
        var startnumberId = dataServiceRegistry.RaceStartNumberDataService.GetModel(startnumberKey).Id;

        var updateStartnumberItem = new RaceStartNumberUpdate()
        {
            Id = startnumberId,
            Auto = data.Auto,
            Turn = data.Turn,
            HasGambling = data.HasGambling,
            FromDirectSource = true,
        };
        var updateCompetitionItem = new CompetitionUpdateStartlistSource()
        {
            Id = competitionId,
            StartlistFromSource = true
        };
        
        await dataServiceRegistry.RaceStartNumberDataService.UpdateAsync(updateStartnumberItem);
        await dataServiceRegistry.CompetitionDataService.UpdateCompetitionStartlistFromSource(updateCompetitionItem);
        _report.StartlistsUpdated++;
    }

    private async Task ParseResults(ResultScrapeData data)
    {
        var racecourseNorm = data.RaceCourse.ToUpper();
        var racecourseExists = dataServiceRegistry.RaceCourseDataService.CheckExists(racecourseNorm);
        if (!racecourseExists)
        {
            _report.ResultsRacecourseNotFound.Add(racecourseNorm);
            return;
        }
        var racecourseId = dataServiceRegistry.RaceCourseDataService.GetModel(racecourseNorm).Id;
        
        var competitionKey = $"{racecourseId}_{data.Date}";
        var competitionExists = dataServiceRegistry.CompetitionDataService.CheckExists(competitionKey);
        if (!competitionExists)
        {
            var res = $"{racecourseNorm} - {data.Date}";
            _report.ResultsCompetitionNotFound.Add(res);
            return;
        }
        var competitionEntity = dataServiceRegistry.CompetitionDataService.GetModel(competitionKey);
        var competitionId = competitionEntity.Id;
        if (competitionEntity.ResultsFromSource) return;
        
        var raceNumber = int.Parse(data.RaceNumber.Split("-")[1]);
        var raceKey = $"{competitionId}_{raceNumber}";
        var raceExists = dataServiceRegistry.RaceDataService.CheckExists(raceKey);
        if (!raceExists)
        {
            var res = $"{racecourseNorm} - {data.Date} - {raceNumber}";
            _report.ResultsRaceNotFound.Add(res);
            return;
        }
        var raceId = dataServiceRegistry.RaceDataService.GetModel(raceKey).Id;
        
        var startnumberKey = $"{raceId}_{data.StartNumber}";
        var startnumberExists = dataServiceRegistry.RaceStartNumberDataService.CheckExists(startnumberKey);
        if (!startnumberExists)
        {
            var res = $"{racecourseNorm} - {data.Date} - {raceNumber} - {data.StartNumber}";
            _report.ResultsStartnumberNotFound.Add(res);
            return;
        }
        var startnumberId = dataServiceRegistry.RaceStartNumberDataService.GetModel(startnumberKey).Id;
        
        var raceResultsExists = dataServiceRegistry.RaceResultDataService.CheckExists(startnumberId);
        if (!raceResultsExists)
        {
            var res = $"{racecourseNorm} - {data.Date} - {raceNumber} -  {data.StartNumber}";
            _report.ResultsNotFound.Add(res);
            return;
        }

        var updateRaceResultsItem = new RaceResultUpdate
        {
            Id = raceId,
            Time = data.Time,
            FromDirectSource = true
        };

        var updateCompetitionItem = new CompetitionUpdateResultSource
        {
            Id = competitionId,
            ResultsFromSource = true
        };
        
        await dataServiceRegistry.RaceResultDataService.UpdateAsync(updateRaceResultsItem);
        await dataServiceRegistry.CompetitionDataService.UpdateCompetitionResultsFromSource(updateCompetitionItem);
        
        _report.ResultsUpdated++;
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
