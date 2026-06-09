using DoctorConnect.DbServices.IServices;
using DoctorConnect.Models;
using DoctorConnect.Repositories;

namespace DoctorConnect.DbServices.Services
{
    public class AvailabilityService : IAvailabilityService
    {
        private readonly IGenericRepository<DoctorAvailability> _repo;

        public AvailabilityService(IGenericRepository<DoctorAvailability> repo)
        {
            _repo = repo;
        }

        public async Task SaveAvailabilityAsync(string doctorId, IEnumerable<DoctorAvailability> availabilities)
        {
            await _repo.ExecuteInTransactionAsync(async () =>
            {
                await _repo.RemoveRangeAsync(x => x.DoctorId == doctorId);
                await _repo.AddRangeAsync(availabilities);
            });
        }

        public async Task<IEnumerable<DoctorAvailability>> GetAllAsync()
        {
            return await _repo.GetAllAsync();
        }

        public async Task<IEnumerable<DoctorAvailability>> GetByIdAsync(string doctorId)
        {
            return await _repo.FindAsync(a => a.DoctorId == doctorId);
        }
    }
}
