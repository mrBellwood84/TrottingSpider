namespace Application.DataServices.Interfaces;

public interface IBaseDataService<TModel>
{
    Task InitCache();
    bool CheckExists(string key);
    TModel GetModel(string key);
    Task CreateAsync(TModel model);
}