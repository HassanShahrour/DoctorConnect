using DoctorConnect.Models;

namespace DoctorConnect.DbServices.IServices
{
    public interface IPatientService
    {
        Task<IEnumerable<Patient>> GetAllAsync();
        Task<Patient> GetByIdAsync(string id);
        Task CreateAsync(Patient patient);
        Task UpdateAsync(Patient patient);
        Task DeleteAsync(string id);
    }
}
