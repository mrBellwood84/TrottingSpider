using Dapper;
using Models.DbModels;
using Models.DbModels.Updates;
using Models.Settings;
using Persistence.Interfaces;

namespace Persistence.Services;

public class RaceResultDbService : BaseDbService<RaceResult>
{
    public RaceResultDbService(DbConnectionStrings connectionStrings) : base(connectionStrings)
    {
        Query = "SELECT * FROM RaceResult";
        InsertCommand =
            "INSERT INTO RaceResult (Id, RaceStartNumberId, Place, Odds, Time, KmTime, RRemark, GRemark, FromDirectSource)" +
            "VALUES (@Id, @RaceStartNumberId, @Place, @Odds, @Time, @KmTime, @RRemark, @GRemark, @FromDirectSource)";
    }
}