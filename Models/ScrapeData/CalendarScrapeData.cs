namespace Models.ScrapeData;
/**
 *  Example data format:
 * 
 *  Date: Torsdag 25 Januar
 *  StartTime: Bergen Travpark kl. 18:55
 *  StartlistHref: https://www.travsport.no/travbaner/bergen-travpark/startlist/2024-01-25
 *  ResultHref: https://www.travsport.no/travbaner/bergen-travpark/results/2024-01-25
 */

public struct CalendarScrapeData
{
    public CalendarScrapeData() { }

    public string Date {  get; init; } = null;
    public string CourseAndTime { get; init; } = null;
    public string StartlistHref { get; init; } = null;
    public string ResultHref  { get; init; } = null;

    public bool StartlistFromSource { get; set; } = false;
    public bool ResultsFromSource { get; set; } = false;
    
}