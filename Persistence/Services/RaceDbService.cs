using Models.DbModels;
using Models.Settings;

namespace Persistence.Services;

public class RaceDbService : BaseDbService<Race>
{
    public RaceDbService(DbConnectionStrings connectionStrings) : base(connectionStrings)
    {
        Query = "SELECT * FROM Race";
        InsertCommand = "INSERT INTO Race (Id, CompetitionId, RaceNumber, Distance, HasGambling) " +
                        "VALUES (@Id, @CompetitionId, @RaceNumber, @Distance, @HasGambling)";
    }
}