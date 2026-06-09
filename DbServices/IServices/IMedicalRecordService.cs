using DoctorConnect.Models;

namespace DoctorConnect.DbServices.IServices
{
    public interface IMedicalRecordService
    {
        Task<IEnumerable<MedicalRecord>> GetAllAsync();
        Task<MedicalRecord> GetByIdAsync(string id);
        Task CreateAsync(MedicalRecord record);
        Task UpdateAsync(MedicalRecord record);
        Task DeleteAsync(string id);
    }
}
