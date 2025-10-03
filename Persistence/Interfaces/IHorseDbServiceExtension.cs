using Models.DbModels;

namespace Persistence.Interfaces;

public interface IHorseDbServiceExtension
{
    Task<List<Horse>> QueryBySourceId(string sourceId);
}