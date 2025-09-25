using Models.DbModels;

namespace Application.CacheServices;

public class RaceCourseCacheService : BaseCacheService<RaceCourse>
{
    public override string CreateKey(RaceCourse model) => model.Name;
}