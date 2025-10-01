using Models.DbModels;

namespace Application.Pipelines.NO.Collection.UpdateStartlistAndResultStep;

public struct UpdateStartlistAndResultReport
{
    public UpdateStartlistAndResultReport() { }

    public int StartlistUpdated { get; set; } = 0;
    public int ResultUpdated { get; set; } = 0;
    public int StartlistNoRacecourse { get; set; } = 0;
    public int StartlistNoCompetition { get; set; } = 0;
    public int StartlistNoRace { get; set; } = 0;
    public int StartlistNoStartnumber { get; set; } = 0;
    public int ResultNoRacecourse { get; set; } = 0;
    public int ResultNoCompetition { get; set; } = 0;
    public int ResultNoRace { get; set; } = 0;
    public int ResultNoStartnumber { get; set; } = 0;
    public int ResultNoResults { get; set; } = 0;


    public void Report()
    {
        Console.WriteLine("\n\n");
        if (StartlistUpdated > 0) AppLogger.LogPositive($"{StartlistUpdated} Startlists updated.");
        if (ResultUpdated > 0) AppLogger.LogPositive($"{ResultUpdated} Results updated.");
        if (StartlistNoRacecourse > 0) AppLogger.LogNegative($"{StartlistNoRacecourse} Startlists no race course.");
        if (StartlistNoCompetition > 0) AppLogger.LogNegative($"{StartlistNoCompetition} Startlists no competition.");
        if (StartlistNoRace > 0) AppLogger.LogNegative($"{StartlistNoRace} Startlists no race.");
        if (StartlistNoStartnumber > 0) AppLogger.LogNegative($"{StartlistNoStartnumber} Startlists no startnumber.");
        if (ResultNoRacecourse > 0) AppLogger.LogNegative($"{ResultNoRacecourse} Results no Racecourse.");
        if (ResultNoCompetition > 0) AppLogger.LogNegative($"{ResultNoCompetition} Results no Competition.");
        if (ResultNoRace > 0) AppLogger.LogNegative($"{ResultNoRace} Results no Race.");
        if (ResultNoStartnumber > 0) AppLogger.LogNegative($"{ResultNoStartnumber} Results no Startnumber.");
        if (ResultNoResults > 0) AppLogger.LogNegative($"{ResultNoResults} Results no Results.");
    }
}