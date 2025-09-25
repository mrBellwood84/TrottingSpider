namespace Models.DbModels;

public class RaceStartNumber
{
    public string Id { get; init; }
    public string RaceId { get; init; }
    public string DriverId { get; init; }
    public string HorseId { get; init; }
    
    public int ProgramNumber  { get; init; }
    public int TrackNumber  { get; init; }
    public string Turn { get; init; }
    public string Auto { get; init; }
    public bool? ForeShoe { get; init; }
    public bool? HindShoe { get; init; }
    public string Cart { get; init; }
    public bool FromDirectSource { get; init; }
}