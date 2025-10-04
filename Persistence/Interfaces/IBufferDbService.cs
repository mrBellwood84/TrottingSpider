using Models.DbModels;

namespace Persistence.Interfaces;

public interface IBufferDbService
{
    Task<List<BufferItem>> GetAllDriversAsync();
    Task AddDriverAsync(string sourceId);
    Task SetDriverCollectedAsync(string sourceId);
    Task<List<BufferItem>> GetAllHorsesAsync();
    Task AddHorseAsync(string sourceId);
    Task SetHorsesCollectedAsync(string sourceId);
}