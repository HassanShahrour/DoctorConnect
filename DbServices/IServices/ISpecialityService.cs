using DoctorConnect.Models;

namespace DoctorConnect.DbServices.IServices
{
    public interface ISpecialityService
    {
        Task<IEnumerable<Specialty>> GetAllAsync();
        Task<Specialty> GetByIdAsync(string id);
        Task CreateAsync(Specialty specialty);
        Task UpdateAsync(Specialty specialty);
        Task DeleteAsync(string id);
    }
}
