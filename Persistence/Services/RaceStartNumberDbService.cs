using Models.DbModels;
using Models.Settings;

namespace Persistence.Services;

public class RaceStartNumberDbService : BaseDbService<RaceStartNumber>
{
    public RaceStartNumberDbService(DbConnectionStrings connectionStrings) : base(connectionStrings)
    {
        Query = "SELECT * FROM RaceStartNumber";
        InsertCommand = "INSERT INTO RaceStartNumber" +
                        "(Id, RaceId, DriverId, HorseId, ProgramNumber, TrackNumber, " +
                        "Distance, Turn, Auto, ForeShoe, HindShoe, Cart, FromDirectSource)" +
                        "VALUES (@Id, @RaceId,  @DriverId, @HorseId, @ProgramNumber, @TrackNumber, @Distance, " +
                        "@Turn, @Auto, @ForeShoe, @HindShoe, @Cart, @FromDirectSource)";
    }
} 