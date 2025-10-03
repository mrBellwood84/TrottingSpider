using Dapper;
using Models.DbModels;
using Models.Settings;
using Persistence.Interfaces;

namespace Persistence.Services.Extensions;

public class DriverDbServiceExtension(DbConnectionStrings dbConnectionStrings)
    : DbConnection(dbConnectionStrings), IDriverDbServiceExtension
{
    private readonly string _queryBySourceId = "SELECT * FROM driver where SourceId = @SourceId";

    public async Task<List<Driver>> QueryBySourceId(string sourceId)
    {
        await using var connection = CreateConnection();
        var result = await connection.QueryAsync<Driver>(_queryBySourceId, new { SourceId = sourceId });
        return result.ToList();
    }
}