using Models.DbModels;

namespace Application.CacheServices;

public class RacecourseCacheService : BaseCacheService<Racecourse>
{
    public override string CreateKey(Racecourse model) => model.Name;
}