using DoctorConnect.Models;

namespace DoctorConnect.DbServices.IServices
{
    public interface IRelativeService
    {
        Task<List<Relative>> GetByPatientIdAsync(string patientId);
        Task<Relative?> GetByIdAsync(string id);
        Task<Relative> CreateAsync(Relative relative);
        Task<Relative?> UpdateAsync(Relative relative);
    }
}
