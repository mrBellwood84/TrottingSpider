namespace Models.DbModels;

public class RaceStartNumber
{
    public string Id { get; init; }
    public string RaceId { get; init; }
    public string DriverId { get; set; }
    public string HorseId { get; set; }
    public int ProgramNumber  { get; init; }
    public int TrackNumber  { get; init; }
    public int Distance { get; init; }
    public string Turn { get; init; }
    public string Auto { get; init; }
    public bool? ForeShoe { get; init; }
    public bool? HindShoe { get; init; }
    public string Cart { get; init; }
    public bool HasGambling  { get; init; } 
    public bool FromDirectSource { get; init; }
    
}