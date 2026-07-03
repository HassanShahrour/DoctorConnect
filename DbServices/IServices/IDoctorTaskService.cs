using DoctorConnect.Models;
using DoctorConnect.ViewModels;

namespace DoctorConnect.DbServices.IServices
{
    public interface IDoctorTaskService
    {
        Task<IEnumerable<DoctorTask>> GetByDoctorIdAsync(string doctorId);
        Task<DoctorTask?> GetByIdAsync(string id);
        Task CreateAsync(DoctorTaskManagementViewModel model);
        Task UpdateAsync(DoctorTaskManagementViewModel model);
        Task UpdateProgressAsync(string id, int progress);
        Task CancelAsync(string id);
        Task UncancelAsync(string id);
        Task DeleteAsync(string id);
    }
}
