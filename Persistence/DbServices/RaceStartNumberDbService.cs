using Models.DbModels;
using Models.Settings;

namespace Persistence.DbServices;

public class RaceStartNumberDbService : BaseDbService<RaceStartNumber>
{
    public RaceStartNumberDbService(DbConnectionStrings connectionStrings) : base(connectionStrings)
    {
        Query = "SELECT * FROM RaceStartNumber";
        InsertCommand = "INSERT INTO RaceStartNumber" +
                        "(Id, RaceId, DriverId, HorseId, ProgramNumber, TrackNumber, Turn, Auto, ForeShoe, HindShoe, Cart, FromDirectSource)" +
                        "VALUES (@Id, @RaceId,  @DriverId, @HorseId, @ProgramNumber, @TrackNumber, @Turn, " +
                        "@Auto, @ForeShoe, @HindShoe, @Cart, @FromDirectSource)";
    }

    public override async Task<Dictionary<string, string>> GetIdDictionary()
    {
        var datalist = await GetAllAsync();
        var result = new Dictionary<string, string>();
        foreach (var item in datalist)
        {
            var key = $"{item.RaceId}_{item.ProgramNumber}";
            result[key] = item.RaceId;
        }
        return result;
    }
}