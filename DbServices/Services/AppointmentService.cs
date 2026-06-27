using DoctorConnect.DbServices.IServices;
using DoctorConnect.Models;
using DoctorConnect.Repositories;
using Microsoft.EntityFrameworkCore;

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

        public async Task<IEnumerable<Appointment>> GetAllAsync(Func<IQueryable<Appointment>, IQueryable<Appointment>>? include = null)
        {
            return await _repo.GetAllAsync(include);
        }

        public async Task<Appointment?> GetByIdAsync(string id, Func<IQueryable<Appointment>, IQueryable<Appointment>>? include = null)
        {
            return await _repo.GetByIdAsync(id, include);
        }

        public async Task UpdateAsync(Appointment appointment)
        {
            await _repo.UpdateAsync(appointment);
            await _repo.SaveChangesAsync();
        }

        public async Task<IEnumerable<Appointment>> GetByDoctorAsync(string doctorId)
        {
            return await _repo.FindAsync(a => EF.Property<string>(a, "DoctorId") == doctorId);
        }

        public async Task<IEnumerable<Appointment>> GetByDoctorInRangeAsync(string doctorId, DateTime start, DateTime end)
        {
            return await _repo.FindAsync(a => EF.Property<string>(a, "DoctorId") == doctorId && ((Appointment)(object)a).AppointmentDate >= start && ((Appointment)(object)a).AppointmentDate <= end);
        }

        public async Task<decimal> GetTotalRevenueForDoctorAsync(string doctorId)
        {
            var all = await GetByDoctorAsync(doctorId);
            return all.Sum(a => a.Fees ?? 0);
        }

        public async Task<int> GetCountByStatusAsync(string doctorId, AppointmentStatus status)
        {
            var all = await GetByDoctorAsync(doctorId);
            return all.Count(a => a.Status == status);
        }
    }
}
