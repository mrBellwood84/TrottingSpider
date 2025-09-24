namespace Models.Record;

public class Race
{
    public string Id { get; set; }
    public string CompetitionId { get; set; }
    public int RaceNumber { get; set; }
    public int Distance { get; set; }
    public bool HasGambling { get; set; }
}