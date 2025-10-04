using Models.DbModels;

namespace Persistence.Interfaces;

public interface IBufferDbService
{
    Task<List<BufferItem>> GetAllDriversAsync();
    Task AddDriverAsync(string sourceId);
    Task AddDriverBulkAsync(List<string> sourceIds);
    Task SetDriverCollectedAsync(string sourceId);
    Task<List<BufferItem>> GetAllHorsesAsync();
    Task AddHorseAsync(string sourceId);
    Task AddHorseBulkAsync(List<string> sourceIds);
    Task SetHorsesCollectedAsync(string sourceId);
}