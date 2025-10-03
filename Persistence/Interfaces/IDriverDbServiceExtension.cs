using Models.DbModels;

namespace Persistence.Interfaces;

public interface IDriverDbServiceExtension
{
    Task<List<Driver>> QueryBySourceId(string sourceId);
}