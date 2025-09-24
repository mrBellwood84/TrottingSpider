namespace Models.ScrapeData;

public struct ResultScrapeData
{
    public string RaceCourse { get; set; }
    public string Date { get; set; }
    public string RaceNumber { get; set; }
    public string StartNumber { get; set; }
    public string DriverSourceId { get; set; }
    public string HorseSourceId  { get; set; }
    public string TrackNumber { get; set; }
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