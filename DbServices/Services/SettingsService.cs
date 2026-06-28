using DoctorConnect.DbServices.IServices;
using DoctorConnect.Models;
using DoctorConnect.Repositories;

namespace DoctorConnect.DbServices.Services
{
    public class SettingsService : ISettingsService
    {
        private readonly IGenericRepository<Settings> _repo;
        public SettingsService(IGenericRepository<Settings> repo)
        {
            _repo = repo;
        }

        public async Task<Settings> GetAsync()
        {
            var all = await _repo.GetAllAsync();
            return all.FirstOrDefault();
        }

        public async Task CreateOrUpdateAsync(Settings settings)
        {
            var existing = (await _repo.GetAllAsync()).FirstOrDefault();
            if (existing == null)
            {
                await _repo.AddAsync(settings);
            }
            else
            {
                existing.NumberOfDaysToDisplay = settings.NumberOfDaysToDisplay;
                await _repo.UpdateAsync(existing);
            }
            await _repo.SaveChangesAsync();
        }
    }
}