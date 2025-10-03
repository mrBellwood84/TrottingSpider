using Dapper;
using Models.DbModels;
using Models.DbModels.Updates;
using Models.Settings;
using Persistence.Interfaces;

namespace Persistence.Services.Extensions;

public class RaceStartNumberDbServiceExtension(DbConnectionStrings dbConnectionStrings) 
    : DbConnection(dbConnectionStrings), IRaceStartNumberDbServiceExtension
{
    private readonly string _insertCommand = "INSERT INTO RaceStartNumber" +
        "(Id, RaceId, DriverId, HorseId, ProgramNumber, TrackNumber, " +
        "Distance, Turn, Auto, ForeShoe, HindShoe, Cart, FromDirectSource)" +
        "VALUES (@Id, @RaceId,  @DriverId, @HorseId, @ProgramNumber, @TrackNumber, @Distance, " +
        "@Turn, @Auto, @ForeShoe, @HindShoe, @Cart, @FromDirectSource)"; 
    
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
    /// Update list of models
    /// </summary>
    public async Task InsertBulkAsync(List<RaceStartNumber> data)
    {
        await using var connection = CreateConnection();
        await using var trans = await connection.BeginTransactionAsync();
        await connection.ExecuteAsync(_insertCommand, data, transaction: trans);
        await trans.CommitAsync();
    }
    
    /// <summary>
    /// Update start number data with driver id
    /// </summary>
    public async Task UpdateDriverAsync(RaceStartNumberUpdateDriver data)
    {
        await using var connection = CreateConnection();
        await connection.ExecuteAsync(_updateDriverCommand, data);
    }
    /// <summary>
    /// Updates a list of data with driver id
    /// </summary>
    public async Task UpdateBulkDriversAsync(List<RaceStartNumberUpdateDriver> data)
    {
        await using var connection = CreateConnection();
        await using var trans = await connection.BeginTransactionAsync();
        await connection.ExecuteAsync(_updateDriverCommand, data, transaction: trans);
        await trans.CommitAsync();
    }
    
    /// <summary>
    /// Update start number data with horse id
    /// </summary>
    public async Task UpdateHorseAsync(RaceStartNumberUpdateHorse values)
    {
        await using var connection = CreateConnection();
        await connection.ExecuteAsync(_updateHorseCommand, values);
    }   
    /// <summary>
    /// Updates a list of data with horse id
    /// </summary>1
    public async Task UpdateHorsesBulkAsync(List<RaceStartNumberUpdateHorse> data)
    {
        await using var connection = CreateConnection();
        await using var trans = await connection.BeginTransactionAsync();
        await connection.ExecuteAsync(_updateHorseCommand,  data, transaction: trans);
        await trans.CommitAsync();
    }
}