namespace Models.DbModels;

public class Race
{
    public string Id { get; init; }
    public string CompetitionId { get; init; }
    public int RaceNumber { get; init; }
    public int Distance { get; init; }
    public bool HasGambling { get; init; } = false;
}