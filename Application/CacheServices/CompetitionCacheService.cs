using Models.DbModels;

namespace Application.CacheServices;

public class CompetitionCacheService : BaseCacheService<Competition>
{
    public override string CreateKey(Competition data) => $"{data.RaceCourseId}_{data.Date}";
}