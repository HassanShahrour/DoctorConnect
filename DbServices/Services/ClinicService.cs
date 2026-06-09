using DoctorConnect.DbServices.IServices;
using DoctorConnect.Models;
using DoctorConnect.Repositories;

namespace DoctorConnect.DbServices.Services
{
    public class ClinicService : IClinicService
    {
        private readonly IGenericRepository<Clinic> _repo;
        public ClinicService(IGenericRepository<Clinic> repo)
        {
            _repo = repo;
        }

        public async Task CreateAsync(Clinic clinic)
        {
            await _repo.AddAsync(clinic);
            await _repo.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var d = await _repo.GetByIdAsync(id);
            if (d != null)
            {
                await _repo.RemoveAsync(d);
                await _repo.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Clinic>> GetAllAsync()
        {
            return await _repo.GetAllAsync();
        }

        public async Task<Clinic> GetByIdAsync(string id)
        {
            return await _repo.GetByIdAsync(id);
        }

        public async Task UpdateAsync(Clinic clinic)
        {
            await _repo.UpdateAsync(clinic);
            await _repo.SaveChangesAsync();
        }
    }
}
