using Models.DbModels.Updates;

namespace Persistence.Interfaces;

public interface IRaceStartNumberDbServiceExtension
{
    Task UpdateDriverAsync(RaceStartNumberUpdateDriver values);
    Task UpdateHorseAsync(RaceStartNumberUpdateHorse values);
    Task UpdateAsync(RaceStartNumberUpdate values);

}