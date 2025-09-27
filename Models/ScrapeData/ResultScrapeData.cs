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
    public string RaceCourse { get; set; }
    public string Date { get; set; }
    public string RaceNumber { get; set; }
    public string StartNumber { get; set; }
    public string DriverSourceId { get; set; }
    public string HorseSourceId  { get; set; }
    public string TrackNumber { get; set; }
    public string Odds { get; set; }
    public string Distance { get; set; }
    public string ForeShoe { get; set; }
    public string HindShoe { get; set; }
    public string Cart { get; set; }
    public string Place { get; set; }
    public string Time { get; set; }
    public string KmTime { get; set; }
    public string RRemark  { get; set; }
    public string GRemark  { get; set; }
    public bool FromDirectSource { get; set; }
}