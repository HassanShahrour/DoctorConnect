using DoctorConnect.Models;

namespace DoctorConnect.DbServices.IServices
{
    public interface IClinicService
    {
        Task<IEnumerable<Clinic>> GetAllAsync();
        Task<Clinic> GetByIdAsync(string id);
        Task CreateAsync(Clinic specialty);
        Task UpdateAsync(Clinic specialty);
        Task DeleteAsync(string id);
    }
}
