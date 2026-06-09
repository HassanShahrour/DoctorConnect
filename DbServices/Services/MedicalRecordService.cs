using DoctorConnect.DbServices.IServices;
using DoctorConnect.Models;
using DoctorConnect.Repositories;

namespace DoctorConnect.DbServices.Services
{
    public class MedicalRecordService : IMedicalRecordService
    {
        private readonly IGenericRepository<MedicalRecord> _repo;

        public MedicalRecordService(IGenericRepository<MedicalRecord> repo)
        {
            _repo = repo;
        }

        public async Task CreateAsync(MedicalRecord record)
        {
            await _repo.AddAsync(record);
            await _repo.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var r = await _repo.GetByIdAsync(id);
            if (r != null)
            {
                await _repo.RemoveAsync(r);
                await _repo.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<MedicalRecord>> GetAllAsync()
        {
            return await _repo.GetAllAsync();
        }

        public async Task<MedicalRecord> GetByIdAsync(string id)
        {
            return await _repo.GetByIdAsync(id);
        }

        public async Task UpdateAsync(MedicalRecord record)
        {
            await _repo.UpdateAsync(record);
            await _repo.SaveChangesAsync();
        }
    }
}
