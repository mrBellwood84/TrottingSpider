using Application.CacheServices.Interfaces;
using Application.DataServices.Interfaces;
using Persistence.Interfaces;

namespace Application.DataServices.Services;

public class BufferDataService(IBufferCacheService cache, IBufferDbService dbService) : IBufferDataService
{
    public HashSet<string> DriverBuffer => cache.DriverBuffer;
    public HashSet<string> HorseBuffer => cache.HorseBuffer;

    public async Task InitBuffers()
    {
        var driverData = await dbService.GetAllDriversAsync();
        var horsesData = await dbService.GetAllHorsesAsync();
        cache.InitBuffers(driverData, horsesData);
    }

    public async Task AddDriverAsync(string sourceId)
    {
        if (cache.AllDrivers.Contains(sourceId)) return;
        await dbService.AddDriverAsync(sourceId);
        cache.AddDriver(sourceId);
    }

    public async Task AddDriverBulkAsync(List<string> sourceIds)
    {
        var filtered = sourceIds.Where(x => !cache.AllDrivers.Contains(x)).ToList();
        if (filtered.Count == 0) return;
        await dbService.AddDriverBulkAsync(filtered);
        foreach (var id in filtered)
            cache.AddDriver(id);
    }

    public async Task RemoveDriverAsync(string sourceId)
    {
        await dbService.SetDriverCollectedAsync(sourceId);
        cache.RemoveDriver(sourceId);
    }

    public async Task AddHorseAsync(string sourceId)
    {
        if (cache.AllHorses.Contains(sourceId)) return;
        await dbService.AddHorseAsync(sourceId);
        cache.AddHorse(sourceId);
    }

    public async Task AddHorseBulkAsync(List<string> sourceIds)
    {
        var filtered = sourceIds.Where(x => !cache.AllHorses.Contains(x)).ToList();
        if (filtered.Count == 0) return;
        await dbService.AddHorseBulkAsync(filtered);
        foreach (var id in filtered)
            cache.AddHorse(id);
    }

    public async Task RemoveHorseAsync(string sourceId)
    {
        await dbService.SetHorsesCollectedAsync(sourceId);
        cache.RemoveHorse(sourceId);
    }
}