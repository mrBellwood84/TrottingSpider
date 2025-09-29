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
}