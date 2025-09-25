using Application.AppLogger;
using Models.DbModels;
using Models.ScrapeData;

namespace Scraping.Processor;

public class ProcessHorseScrapeData
{
    private readonly AppLogger _logger = new AppLogger();

    public Horse Process(HorseScrapeData data)
    {
        var horse = new Horse
        {
            Id = Guid.NewGuid().ToString(),
            SourceId = data.SourceId,
            Name = data.Name,
            Sex = data.Sex,
            YearOfBirth = _parseYearOfBirth(data.YearOfBirth, data.SourceId)
        };
        return horse;
    }
    
    private int _parseYearOfBirth(string yearOfBirth, string sourceId)
    {
        try
        {
            return int.Parse(yearOfBirth);
        }
        catch (FormatException)
        {
            _logger.LogError($"ProcessHorseScrapeData: Invalid year of birth '{yearOfBirth}', horse: {sourceId}");
            _logger.LogInformation($"ProcessHorseScrapeData: Year of birth set to 1900 for horse: '{sourceId}'");
            return 1900;
        }
    }
    
}