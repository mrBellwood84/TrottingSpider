namespace Models;

public class RaceStartNumber
{
    public string Id { get; set; }
    public string RaceId { get; set; }
    public string DriverId { get; set; }
    public string HorseId { get; set; }
    
    public int ProgramNumber  { get; set; }
    public int TrackNumber  { get; set; }
    public string Turn { get; set; }
    public string Auto { get; set; }
    public bool? ForeShoe { get; set; }
    public bool? HindShoe { get; set; }
    public string Cart { get; set; }
    public bool FromDirectSource { get; set; }
}