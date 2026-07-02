using DoctorConnect.Models;

namespace DoctorConnect.DbServices.IServices
{
    public interface IServiceService
    {
        Task<IEnumerable<Service>> GetDoctorServicesAsync(string doctorId);
        Task<IEnumerable<Service>> GetClinicServicesAsync(string clinicId);
        Task<Service?> GetByIdAsync(string id);
        Task CreateForDoctorAsync(Service service);
        Task CreateForClinicAsync(Service service);
        Task UpdateAsync(Service service);
        Task DeleteAsync(string id);
    }
}
