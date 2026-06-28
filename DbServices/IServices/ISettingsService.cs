using DoctorConnect.Models;

namespace DoctorConnect.DbServices.IServices
{
    public interface ISettingsService
    {
        Task<Settings> GetAsync();
        Task CreateOrUpdateAsync(Settings settings);
    }
}