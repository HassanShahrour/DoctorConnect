using DoctorConnect.DbServices.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DoctorConnect.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly IDashboardService _dashboard;

        public DashboardController(IDashboardService dashboard)
        {
            _dashboard = dashboard;
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> Admin()
        {
            var model = new
            {
                TotalDoctors = await _dashboard.GetTotalDoctorsAsync(),
                TotalPatients = await _dashboard.GetTotalPatientsAsync(),
                TotalAppointments = await _dashboard.GetTotalAppointmentsAsync(),
                CompletedAppointments = await _dashboard.GetCompletedAppointmentsAsync(),
                PendingAppointments = await _dashboard.GetPendingAppointmentsAsync()
            };
            return View(model);
        }

        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> Doctor()
        {
            // Simple placeholders - services can be extended for doctor-specific stats
            var model = new
            {
                TodaysAppointments = 0,
                UpcomingAppointments = 0,
                PatientCount = await _dashboard.GetTotalPatientsAsync()
            };
            return View(model);
        }

        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> Patient()
        {
            var model = new
            {
                UpcomingAppointments = 0,
                AppointmentHistory = 0
            };
            return View(model);
        }
    }
}
