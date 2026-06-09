using DoctorConnect.Models;

namespace DoctorConnect.DbServices.IServices
{
    public interface IAvailabilityService
    {
        Task<IEnumerable<DoctorAvailability>> GetAllAsync();
        Task<IEnumerable<DoctorAvailability>> GetByIdAsync(string doctorId);
        Task SaveAvailabilityAsync(string doctorId, IEnumerable<DoctorAvailability> availability);
    }
}
