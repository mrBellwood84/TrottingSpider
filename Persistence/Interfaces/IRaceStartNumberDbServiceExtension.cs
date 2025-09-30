using Models.DbModels.Updates;

namespace Persistence.Services.Extensions;

public interface IRaceStartNumberDbServiceExtension
{
    Task UpdateDriverAsync(RaceStartNumberUpdateDriver values);
    Task UpdateHorseAsync(RaceStartNumberUpdateHorse values);
}