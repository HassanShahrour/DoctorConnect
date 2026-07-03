using DoctorConnect.Models;
using DoctorConnect.ViewModels;

namespace DoctorConnect.DbServices.IServices
{
    public interface IDoctorService
    {
        Task<IEnumerable<Doctor>> GetAllAsync();
        Task<Doctor> GetByIdAsync(string id);
        Task<Doctor?> GetByUserIdAsync(string userId);
        Task CreateAsync(Doctor doctor);
        Task UpdateAsync(EditDoctorViewModel doctor);
        Task DeleteAsync(string id);
        Task<Doctor?> GetWithServicesAsync(string id);
    }
}
