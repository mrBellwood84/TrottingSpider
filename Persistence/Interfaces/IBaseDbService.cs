namespace Persistence.Interfaces;

public interface IBaseDbService<TModel>
{
    Task<List<TModel>> GetAllAsync();
    Task CreateAsync(TModel model);
}