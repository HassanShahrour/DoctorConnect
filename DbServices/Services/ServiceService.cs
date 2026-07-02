using DoctorConnect.DbServices.IServices;
using DoctorConnect.Models;
using DoctorConnect.Repositories;

namespace DoctorConnect.DbServices.Services
{
    public class ServiceService : IServiceService
    {
        private readonly IGenericRepository<Service> _repo;

        public ServiceService(IGenericRepository<Service> repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<Service>> GetDoctorServicesAsync(string doctorId)
        {
            var services = await _repo.FindAsync(s => s.DoctorId == doctorId);
            return services.OrderBy(s => s.Name).ThenByDescending(s => s.CreatedAt);
        }

        public async Task<IEnumerable<Service>> GetClinicServicesAsync(string clinicId)
        {
            var services = await _repo.FindAsync(s => s.ClinicId == clinicId);
            return services.OrderBy(s => s.Name).ThenByDescending(s => s.CreatedAt);
        }

        public async Task<Service?> GetByIdAsync(string id)
        {
            return await _repo.GetByIdAsync(id);
        }

        public async Task CreateForDoctorAsync(Service service)
        {
            service.ClinicId = null;
            await _repo.AddAsync(service);
            await _repo.SaveChangesAsync();
        }

        public async Task CreateForClinicAsync(Service service)
        {
            service.DoctorId = null;
            await _repo.AddAsync(service);
            await _repo.SaveChangesAsync();
        }

        public async Task UpdateAsync(Service service)
        {
            await _repo.UpdateAsync(service);
            await _repo.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var service = await _repo.GetByIdAsync(id);
            if (service == null)
            {
                return;
            }

            await _repo.RemoveAsync(service);
            await _repo.SaveChangesAsync();
        }
    }
}
