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
    public string Date;
    public string CourseAndTime;
    public string StartlistHref;
    public string ResultHref;
}