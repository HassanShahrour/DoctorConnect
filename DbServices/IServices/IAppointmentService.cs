using DoctorConnect.Models;

namespace DoctorConnect.DbServices.IServices
{
    public interface IAppointmentService
    {
        Task<IEnumerable<Appointment>> GetAllAsync(Func<IQueryable<Appointment>, IQueryable<Appointment>>? include = null);
        Task<Appointment?> GetByIdAsync(string id, Func<IQueryable<Appointment>, IQueryable<Appointment>>? include = null);
        Task CreateAsync(Appointment appointment);
        Task UpdateAsync(Appointment appointment);
        Task DeleteAsync(string id);

        // New methods
        Task<IEnumerable<Appointment>> GetByDoctorAsync(string doctorId);
        Task<IEnumerable<Appointment>> GetByDoctorInRangeAsync(string doctorId, DateTime start, DateTime end);
        Task<decimal> GetTotalRevenueForDoctorAsync(string doctorId);
        Task<int> GetCountByStatusAsync(string doctorId, AppointmentStatus status);
    }
}
