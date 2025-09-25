using Models.DbModels;
using Models.Settings;

namespace Persistence.DbServices;

public class RaceResultDbService : BaseDbService<RaceResult>
{
    public RaceResultDbService(DbConnectionStrings connectionStrings) : base(connectionStrings)
    {
        Query = "SELECT * FROM RaceResult";
        InsertCommand =
            "INSERT INTO RaceResult (Id, RaceStartNumberId, Place, Time, KmTime, RRemark, GRemark, FromDirectSource)" +
            "VALUES (@Id, @RaceStartNumberId, @Place, @Time, @KmTime, @RRemark, @GRemark, @FromDirectSource)";
    }
}