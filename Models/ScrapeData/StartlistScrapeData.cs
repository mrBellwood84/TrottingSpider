namespace Models.ScrapeData;
/**
  -- Startlist scrape data example
RaceCourse: Jarlsberg Travbane
Date: 2025-09-21
RaceNumber: race-1
StartNumber: 1
DriverSourceId: 30051177
HorseSourceId: 25000113366177W
TrackNumber: 1
ForeShoe: showForeShoeOn
HindShoe: showHindShoeOn
Turn: 1.11,9v
Auto: 1.09,8a
Distance: 1609
Cart: Am
HasGambling: True
 */

public struct StartlistScrapeData
{
    public string RaceCourse { get; set; }
    public string Date { get; set; }
    public string RaceNumber { get; set; }
    public string StartNumber { get; set; }
    public string DriverSourceId { get; set; }
    public string HorseSourceId { get; set; }
    public string TrackNumber { get; set; }
    public string ForeShoe { get; set; }
    public string HindShoe { get; set; }
    public string Turn { get; set; }
    public string Auto { get; set; }
    public string Distance { get; set; }
    public string Cart { get; set; }
    public bool HasGambling { get; set; }
}