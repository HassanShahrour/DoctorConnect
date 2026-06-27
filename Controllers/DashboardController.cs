using DoctorConnect.DbServices.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        public IActionResult Admin()
        {
            var model = new
            {
                TotalDoctors = _dashboard.GetTotalDoctorsAsync().Result,
                TotalPatients = _dashboard.GetTotalPatientsAsync().Result,
                TotalAppointments = _dashboard.GetTotalAppointmentsAsync().Result,
                CompletedAppointments = _dashboard.GetCompletedAppointmentsAsync().Result,
                PendingAppointments = _dashboard.GetPendingAppointmentsAsync().Result
            };
            return View(model);
        }

        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> Doctor()
        {
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
