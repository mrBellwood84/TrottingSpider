using Models.DbModels;

namespace Application.DataServices.Interfaces;

public interface IDriverLicenseDataService
{
    Dictionary<string, DriverLicense> GetFullCache();
    Task InitCache();
    bool CheckExists(string key);
    DriverLicense GetModel(string key);
    Task AddAsync(DriverLicense model);
    Task AddAsync(List<DriverLicense> models);

}