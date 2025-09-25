using System.Reflection.Metadata.Ecma335;
using Models.DbModels;

namespace Application.CacheServices;

public class RaceCacheService : BaseCacheService<Race>
{
    public override string CreateKey(Race model) => $"{model.CompetitionId}_{model.RaceNumber}";
}