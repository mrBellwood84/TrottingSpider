namespace Models.ScrapeData;

/**
  -- Results scrape data example
RaceCourse: Jarlsberg Travbane
Date: 2025-09-21
RaceNumber: race-1
StartNumber: 2
DriverSourceId: 2209
HorseSourceId: 578001020195279
TrackNumber:
Odds: 13
Distance: 1609
ForeShoe: showForeShoeOn
HindShoe: showHindShoeOn
Cart: Amerikansk
Place: 1
Time: 1.56,4
KmTime: 12,3a
RRemark:
GRemark:
FromDirectSource: True
 */

public struct ResultScrapeData
{
    public string RaceCourse { get; init; }
    public string Date { get; init; }
    public string RaceNumber { get; init; }
    public string StartNumber { get; init; }
    public string DriverSourceId { get; init; }
    public string HorseSourceId  { get; init; }
    public string TrackNumber { get; init; }
    public string Odds { get; init; }
    public string Distance { get; init; }
    public string ForeShoe { get; init; }
    public string HindShoe { get; init; }
    public string Cart { get; init; }
    public string Place { get; init; }
    public string Time { get; init; }
    public string KmTime { get; init; }
    public string RRemark  { get; init; }
    public string GRemark  { get; init; }
    public bool FromDirectSource { get; init; }
}