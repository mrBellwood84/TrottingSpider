using Dapper;
using Models.DbModels.Updates;
using Models.Settings;
using Persistence.Interfaces;

namespace Persistence.Services.Extensions;

public class RaceResultExtension(DbConnectionStrings dbConnectionStrings)
    : DbConnection(dbConnectionStrings), IRaceResultExtension
{
    private readonly string _updateCommand = 
        "UPDATE RaceResult SET " +
        "Time = @Time, FromDirectSource = @FromDirectSource " +
        "WHERE Id = @Id";

    public async Task UpdateAsync(RaceResultUpdate values)
    {
        await using var connection = CreateConnection();
        await connection.ExecuteAsync(_updateCommand, values);
    }
}