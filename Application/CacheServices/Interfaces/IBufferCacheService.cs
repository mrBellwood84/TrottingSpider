using Models.DbModels;

namespace Application.CacheServices.Interfaces;

public interface IBufferCacheService
{
    HashSet<string> AllDrivers { get; set; }
    HashSet<string> AllHorses { get; set; }
    HashSet<string> DriverBuffer { get; set; }
    HashSet<string> HorseBuffer { get; set; }
    void InitBuffers(List<BufferItem> driverData, List<BufferItem> horseData);
    void AddDriver(string sourceId);
    void AddHorse(string sourceId);
    void RemoveDriver(string sourceId);
    void RemoveHorse(string sourceId);
}