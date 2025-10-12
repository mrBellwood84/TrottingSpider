namespace Application.Pipelines.NO.Collection.UpdateStartlistAndResultStep;

public struct UpdateStartlistAndResultReport
{
    public UpdateStartlistAndResultReport() { }

    public int Startlists { get; set; } = 0;
    public int Results { get; set; } = 0;
    
    public int StartlistsUpdated { get; set; } = 0;
    public int ResultsUpdated { get; set; } = 0;
    
    public List<string> StartlistRacecourseNotFound { get; } = [];
    public List<string> StartlistCompetitionNotFound { get; } = [];
    public List<string> StartlistRaceNotFound { get; } = [];
    public List<string> StartlistStartnumberNotFound { get; } = [];
    
    public List<string> ResultsRacecourseNotFound { get; } = [];
    public List<string> ResultsCompetitionNotFound { get; } = [];
    public List<string> ResultsRaceNotFound { get; } = [];
    public List<string> ResultsStartnumberNotFound { get; } = [];
    public List<string> ResultsNotFound { get; } = [];


    public void PrintReport()
    {
        Console.WriteLine("\n\n");
        if (Startlists > 0) AppLogger.AppLogger.LogNeutral($"Startlists - {Startlists}");
        if (Results > 0) AppLogger.AppLogger.LogNeutral($"Results - {Results}");
        
        if (StartlistsUpdated > 0) AppLogger.AppLogger.LogPositive($"Startlists Updated - {StartlistsUpdated}");
        if (ResultsUpdated > 0) AppLogger.AppLogger.LogPositive($"Results Updated - {ResultsUpdated}");

        if (StartlistRacecourseNotFound.Count > 0)
        {
            AppLogger.AppLogger.LogNegative($"Startlists Racecourse Not Found: {StartlistRacecourseNotFound.Count}");
            foreach (var racecourseNotFound in StartlistRacecourseNotFound)
                AppLogger.AppLogger.LogNegative($"\t{racecourseNotFound}");
        }

        if (StartlistCompetitionNotFound.Count > 0)
        {
            AppLogger.AppLogger.LogNegative($"Startlists Competition not found: {StartlistCompetitionNotFound.Count}");
            foreach (var competitionNotFound in StartlistCompetitionNotFound)
                AppLogger.AppLogger.LogNegative($"\t{competitionNotFound}");
        }

        if (StartlistRaceNotFound.Count > 0)
        {
            AppLogger.AppLogger.LogNegative($"Startlists Race not found: {StartlistRaceNotFound.Count}");
            foreach (var raceNotFound in StartlistRaceNotFound) 
                AppLogger.AppLogger.LogNegative($"\t{raceNotFound}");
        }

        if (StartlistStartnumberNotFound.Count > 0)
        {
            AppLogger.AppLogger.LogNegative($"Startlists Startnumber not found: {StartlistStartnumberNotFound.Count}");
            foreach (var startnumberNotFound in StartlistStartnumberNotFound)
                AppLogger.AppLogger.LogNegative($"\t{startnumberNotFound}");
        }
            

        if (ResultsRacecourseNotFound.Count > 0)
        {
            AppLogger.AppLogger.LogNegative($"Results Racecourse Not Found: {ResultsRacecourseNotFound.Count}");
            foreach (var racecourseNotFound in ResultsRacecourseNotFound)
                AppLogger.AppLogger.LogNegative($"\t{racecourseNotFound}");
        }

        if (ResultsCompetitionNotFound.Count > 0)
        {
            AppLogger.AppLogger.LogNegative($"Results Competition not found: {ResultsCompetitionNotFound.Count}");
            foreach (var competitionNotFound in ResultsCompetitionNotFound)
                AppLogger.AppLogger.LogNegative($"\t{competitionNotFound}");
        }

        if (ResultsRaceNotFound.Count > 0)
        {
            AppLogger.AppLogger.LogNegative($"Results Racecourse not found: {ResultsRaceNotFound.Count}");
            foreach (var raceNotFound in ResultsRaceNotFound)
                AppLogger.AppLogger.LogNegative($"\t{raceNotFound}");
        }

        if (ResultsStartnumberNotFound.Count > 0)
        {
            AppLogger.AppLogger.LogNegative($"Results Startnumber not found: {ResultsStartnumberNotFound.Count}");
            foreach (var startnumberNotFound in ResultsStartnumberNotFound)
                AppLogger.AppLogger.LogNegative($"\t{startnumberNotFound}");
        }

        if (ResultsNotFound.Count > 0)
        {
            AppLogger.AppLogger.LogNegative($"Results not found: {ResultsNotFound.Count}");
            foreach (var resultNotFound in ResultsNotFound)
                AppLogger.AppLogger.LogNegative($"\t{resultNotFound}");
        }
    }
}