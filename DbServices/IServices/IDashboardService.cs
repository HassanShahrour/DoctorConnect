using DoctorConnect.Models;
using System.Threading.Tasks;

namespace DoctorConnect.DbServices.IServices
{
    public interface IDashboardService
    {
        Task<int> GetTotalDoctorsAsync();
        Task<int> GetTotalPatientsAsync();
        Task<int> GetTotalAppointmentsAsync();
        Task<int> GetCompletedAppointmentsAsync();
        Task<int> GetPendingAppointmentsAsync();
    }
}
