using Application.CacheServices.Interfaces;
using Models.DbModels;

namespace Application.CacheServices.Services;

public class BufferCacheService : IBufferCacheService
{
    public HashSet<string> AllDrivers { get; set; } = [];
    public HashSet<string> AllHorses { get; set; } = [];
    public HashSet<string> DriverBuffer { get; set; } = [];
    public HashSet<string> HorseBuffer { get; set; } = [];

    public void InitBuffers(List<BufferItem> driverData, List<BufferItem> horseData)
    {
        foreach (var driver in driverData)
        {
            AllDrivers.Add(driver.SourceId);
            if (!driver.Collected) DriverBuffer.Add(driver.SourceId);
        }

        foreach (var horse in horseData)
        {
            AllHorses.Add(horse.SourceId);
            if (!horse.Collected) HorseBuffer.Add(horse.SourceId);
        }
    }

    public void AddDriver(string sourceId)
    {
        AllDrivers.Add(sourceId);
        DriverBuffer.Add(sourceId);
    }

    public void AddHorse(string sourceId)
    {
        AllHorses.Add(sourceId);
        HorseBuffer.Add(sourceId);
    }
    public void RemoveDriver(string sourceId) =>  DriverBuffer.Remove(sourceId);
    public void RemoveHorse(string sourceId) =>  HorseBuffer.Remove(sourceId);

}