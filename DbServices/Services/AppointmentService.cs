using DoctorConnect.DbServices.IServices;
using DoctorConnect.Models;
using DoctorConnect.Repositories;

namespace DoctorConnect.DbServices.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IGenericRepository<Appointment> _repo;

        public AppointmentService(IGenericRepository<Appointment> repo)
        {
            _repo = repo;
        }

        public async Task CreateAsync(Appointment appointment)
        {
            await _repo.AddAsync(appointment);
            await _repo.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var a = await _repo.GetByIdAsync(id);
            if (a != null)
            {
                await _repo.RemoveAsync(a);
                await _repo.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Appointment>> GetAllAsync()
        {
            return await _repo.GetAllAsync();
        }

        public async Task<Appointment> GetByIdAsync(string id)
        {
            return await _repo.GetByIdAsync(id);
        }

        public async Task UpdateAsync(Appointment appointment)
        {
            await _repo.UpdateAsync(appointment);
            await _repo.SaveChangesAsync();
        }
    }
}
