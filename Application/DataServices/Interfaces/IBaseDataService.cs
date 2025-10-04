namespace Application.DataServices;

public interface IBaseDataService<TModel>
{
    Task InitCache();
    bool CheckExists(string key);
    TModel GetModel(string key);
    Task AddAsync(TModel model);
    Task AddBulkAsync(List<TModel> models);
}