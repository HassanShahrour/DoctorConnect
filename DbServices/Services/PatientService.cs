using DoctorConnect.DbServices.IServices;
using DoctorConnect.Models;
using DoctorConnect.Repositories;

namespace DoctorConnect.DbServices.Services
{
    public class PatientService : IPatientService
    {
        private readonly IGenericRepository<Patient> _repo;

        public PatientService(IGenericRepository<Patient> repo)
        {
            _repo = repo;
        }

        public async Task CreateAsync(Patient patient)
        {
            await _repo.AddAsync(patient);
            await _repo.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var p = await _repo.GetByIdAsync(id);
            if (p != null)
            {
                await _repo.RemoveAsync(p);
                await _repo.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Patient>> GetAllAsync()
        {
            return await _repo.GetAllAsync();
        }

        public async Task<Patient> GetByIdAsync(string id)
        {
            return await _repo.GetByIdAsync(id);
        }

        public async Task UpdateAsync(Patient patient)
        {
            await _repo.UpdateAsync(patient);
            await _repo.SaveChangesAsync();
        }
    }
}
