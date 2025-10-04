namespace Application.DataServices.Interfaces;

public interface IBufferDataService
{
    HashSet<string> DriverBuffer { get; }
    HashSet<string> HorseBuffer { get; }
    Task InitBuffers();
    Task AddDriverAsync(string sourceId);
    Task RemoveDriverAsync(string sourceId);
    Task AddHorseAsync(string sourceId);
    Task RemoveHorseAsync(string sourceId);
}