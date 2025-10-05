using Models.DbModels;
using Models.ScrapeData;

namespace Scraping.Processors;

public class ProcessHorseScrapeData
{

    public Horse Process(HorseScrapeData data)
    {
        var horse = new Horse
        {
            Id = Guid.NewGuid().ToString(),
            SourceId = data.SourceId,
            Name = data.Name,
            Sex = data.Sex,
            YearOfBirth = _parseYearOfBirth(data.YearOfBirth)
        };
        return horse;
    }
    
    private int _parseYearOfBirth(string yearOfBirth)
    {
        try
        {
            return int.Parse(yearOfBirth);
        }
        catch (FormatException)
        {
            return 1900;
        }
    }
}