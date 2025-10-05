using Dapper;
using Models.DbModels.Updates;
using Models.Settings;
using Persistence.Interfaces;

namespace Persistence.Services.Extensions;

public class RaceStartNumberDbServiceExtension(DbConnectionStrings dbConnectionStrings) 
    : DbConnection(dbConnectionStrings), IRaceStartNumberDbServiceExtension
{
    
    private readonly string _updateCommand = 
        "UPDATE RaceStartNumber SET " +
        "Turn = @Turn, Auto = @Auto, HasGambling = @HasGambling, FromDirectSource = @FromDirectSource " +
        "WHERE Id = @Id";
    
    private readonly string _updateDriverCommand = 
        "UPDATE RaceStartNumber SET DriverId = @DriverId WHERE Id = @Id";

    private readonly string _updateHorseCommand = 
        "UPDATE RaceStartNumber SET HorseId = @HorseId  WHERE Id = @Id";
    
    
    /// <summary>
    /// Update single model
    /// </summary>
    public async Task UpdateAsync(RaceStartNumberUpdate data)
    {
        await using var connection = CreateConnection();
        await connection.ExecuteAsync(_updateCommand, data);
    }

    /// <summary>
    /// Updates a list of data with driver id
    /// </summary>
    public async Task BulkUpdateDriversAsync(List<RaceStartNumberUpdateDriver> data)
    {
        await using var connection = CreateConnection();
        await using var trans = await connection.BeginTransactionAsync();
        await connection.ExecuteAsync(_updateDriverCommand, data, transaction: trans);
        await trans.CommitAsync();
    }

    /// <summary>
    /// Updates a list of data with horse id
    /// </summary>1
    public async Task BulkUpdateHorsesAsync(List<RaceStartNumberUpdateHorse> data)
    {
        await using var connection = CreateConnection();
        await using var trans = await connection.BeginTransactionAsync();
        await connection.ExecuteAsync(_updateHorseCommand,  data, transaction: trans);
        await trans.CommitAsync();
    }
}