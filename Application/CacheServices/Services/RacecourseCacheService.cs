using Models.DbModels;

namespace Application.CacheServices.Services;

public class RacecourseCacheService : BaseCacheService<Racecourse>
{
    public override string CreateKey(Racecourse model) => model.Name;
}