namespace Models.ScrapeData;

/** Horse Scrape data example:
    SourceId: 75200211S191416
    Name: Premium Tooma
    YearOfBirth: 2019
    Sex: Vallak
*/

public struct HorseScrapeData
{
    public string SourceId {get;init;}
    public string Name { get; init; }
    public string YearOfBirth { get; init; }
    public string Sex { get; init; }
}