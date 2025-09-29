using Models.DbModels;

namespace Application.CacheServices.Services;

public class RaceCacheService : BaseCacheService<Race>
{
    public override string CreateKey(Race model) => $"{model.CompetitionId}_{model.RaceNumber}";
}