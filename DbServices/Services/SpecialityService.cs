using DoctorConnect.DbServices.IServices;
using DoctorConnect.Models;
using DoctorConnect.Repositories;

namespace DoctorConnect.DbServices.Services
{
    public class SpecialityService : ISpecialityService
    {
        private readonly IGenericRepository<Specialty> _repo;
        public SpecialityService(IGenericRepository<Specialty> repo)
        {
            _repo = repo;
        }

        public async Task CreateAsync(Specialty specialty)
        {
            await _repo.AddAsync(specialty);
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

        public async Task<IEnumerable<Specialty>> GetAllAsync()
        {
            return await _repo.GetAllAsync();
        }

        public async Task<Specialty> GetByIdAsync(string id)
        {
            return await _repo.GetByIdAsync(id);
        }

        public async Task UpdateAsync(Specialty specialty)
        {
            await _repo.UpdateAsync(specialty);
            await _repo.SaveChangesAsync();
        }
    }
}
