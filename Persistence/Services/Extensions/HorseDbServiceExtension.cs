using Dapper;
using Models.DbModels;
using Models.Settings;
using Persistence.Interfaces;

namespace Persistence.Services.Extensions;

public class HorseDbServiceExtension(DbConnectionStrings dbConnectionStrings) 
    : DbConnection(dbConnectionStrings), IHorseDbServiceExtension
{
    private const string _queryBySourceId = "SELECT * FROM horse where SourceId = @SourceId";

    public async Task<List<Horse>> QueryBySourceId(string sourceId)
    {
        await using var connection = CreateConnection();
        var result = await connection.QueryAsync<Horse>(_queryBySourceId, new { SourceId = sourceId });
        return result.ToList();
    }
}