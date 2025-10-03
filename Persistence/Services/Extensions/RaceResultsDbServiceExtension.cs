using Dapper;
using Models.DbModels;
using Models.DbModels.Updates;
using Models.Settings;
using Persistence.Interfaces;

namespace Persistence.Services.Extensions;

public class RaceResultsDbServiceExtension(DbConnectionStrings dbConnectionStrings) 
    : DbConnection(dbConnectionStrings), IRaceResultsDbServiceExtension
{
    private readonly string _updateCommand = 
        "UPDATE RaceResult SET " +
        "Time = @Time, FromDirectSource = @FromDirectSource " +
        "WHERE Id = @Id";
    
    public async Task InsertBulkAsync(List<RaceResult> data)
    {
        await using var connection = CreateConnection();
        await using var trans = await connection.BeginTransactionAsync();
        await connection.ExecuteAsync(_updateCommand, data, trans);
        await trans.CommitAsync();
    }
    
    public async Task UpdateAsync(RaceResultUpdate data)
    {
        await using var connection = CreateConnection();
        await connection.ExecuteAsync(_updateCommand, data);
    }
}