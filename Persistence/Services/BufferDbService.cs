using System.Security.Permissions;
using Dapper;
using Models.DbModels;
using Models.Settings;
using Persistence.Interfaces;

namespace Persistence.Services;

public class BufferDbService(DbConnectionStrings dbConnectionStrings) 
    : DbConnection(dbConnectionStrings), IBufferDbService
{
    private readonly string _selectAllDrivers = "SELECT * FROM driverBuffer";
    private readonly string _selectAllHorses = "SELECT * FROM horseBuffer";
    
    private readonly string _insertNewDriver = "INSERT INTO driverBuffer (id, sourceId) VALUES (@id, @sourceId)";
    private readonly string _insertNewHorse = "INSERT INTO horseBuffer (id, sourceId) VALUES (@id, @sourceId)";
    
    private readonly string _setDriverCollected = "UPDATE driverBuffer SET collected = true WHERE SourceId = @sourceId";
    private readonly string _setHorseCollected =  "UPDATE horseBuffer SET collected = true WHERE SourceId = @sourceId";

    
    public async Task<List<BufferItem>> GetAllDriversAsync()
    {
        await using var connection = CreateConnection();
        var result = await connection.QueryAsync<BufferItem>(_selectAllDrivers);
        return result.ToList();

    }
    public async Task AddDriverAsync(string sourceId)
    {
        var data = new BufferItem
        {
            Id = Guid.NewGuid().ToString(),
            SourceId = sourceId
        };
        await using var connection = CreateConnection();
        await connection.ExecuteAsync(_insertNewDriver, data);
    }

    public async Task AddDriverBulkAsync(List<string> sourceIds)
    {
        var data = sourceIds.Select(sourceId => 
            new BufferItem { Id = Guid.NewGuid().ToString(), SourceId = sourceId }).ToList();
        await using var connection = CreateConnection();
        await using var trans = await connection.BeginTransactionAsync();
        await connection.ExecuteAsync(_insertNewDriver, data, transaction: trans);
        await trans.CommitAsync();
    }
    public async Task SetDriverCollectedAsync(string sourceId)
    {
        var data = new  BufferItem { SourceId = sourceId };
        await using var connection = CreateConnection();
        await connection.ExecuteAsync(_setDriverCollected, data);
    }

    
    public async Task<List<BufferItem>> GetAllHorsesAsync()
    {
        await using var connection = CreateConnection();
        var result = await connection.QueryAsync<BufferItem>(_selectAllHorses);
        return result.ToList();
    }
    public async Task AddHorseAsync(string sourceId)
    {
        var data = new BufferItem
        {
            Id = Guid.NewGuid().ToString(),
            SourceId = sourceId
        };
        await using var connection = CreateConnection();
        await connection.ExecuteAsync(_insertNewHorse, data);
    }
    public async Task AddHorseBulkAsync(List<string> sourceIds)
    {
        var data = sourceIds.Select(sourceId =>
            new BufferItem { Id = Guid.NewGuid().ToString(), SourceId = sourceId }).ToList();
        await using var connection = CreateConnection();
        await using var trans = await connection.BeginTransactionAsync();
        await connection.ExecuteAsync(_insertNewHorse, data, transaction: trans);
        await trans.CommitAsync();
    }
    public async Task SetHorsesCollectedAsync(string sourceId)
    {
        var data = new  BufferItem { SourceId = sourceId };
        await using var connection = CreateConnection();
        await connection.ExecuteAsync(_setHorseCollected, data);
    }
}