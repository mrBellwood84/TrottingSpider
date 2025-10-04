namespace Persistence.Interfaces;

public interface IBaseDbService<TModel>
{
    Task<List<TModel>> GetAllAsync();
    Task InsertAsync(TModel model);
    Task BulkInsertAsync(List<TModel> models);
}