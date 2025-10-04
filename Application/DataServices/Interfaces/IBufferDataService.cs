namespace Application.DataServices.Interfaces;

public interface IBufferDataService
{
    HashSet<string> DriverBuffer { get; }
    HashSet<string> HorseBuffer { get; }
    Task InitBuffers();
    Task AddDriverAsync(string sourceId);
    Task AddDriverBulkAsync(List<string> sourceIds);
    Task RemoveDriverAsync(string sourceId);
    Task AddHorseAsync(string sourceId);
    Task AddHorseBulkAsync(List<string> sourceIds);
    Task RemoveHorseAsync(string sourceId);
}