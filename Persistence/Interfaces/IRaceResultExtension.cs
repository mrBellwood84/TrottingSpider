using Models.DbModels.Updates;

namespace Persistence.Interfaces;

public interface IRaceResultExtension
{
    Task UpdateAsync(RaceResultUpdate values);
}