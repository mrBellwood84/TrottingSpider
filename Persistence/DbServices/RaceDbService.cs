using Models.DbModels;
using Models.Settings;

namespace Persistence.DbServices;

public class RaceDbService : BaseDbService<Race>
{
    public RaceDbService(DbConnectionStrings connectionStrings) : base(connectionStrings)
    {
        Query = "SELECT * FROM Race";
        InsertCommand = "INSERT INTO Race (Id, CompetitionId, RaceNumber, Distance, HasGambling)";
    }

    public override async Task<Dictionary<string, string>> GetIdDictionary()
    {
        var datalist = await GetAllAsync();
        var result = new Dictionary<string, string>();
        foreach (var item in datalist)
        {
            var key = $"{item.CompetitionId}_{item.RaceNumber}";
            result[key] = item.Id;
        }
        return result;
    }
}