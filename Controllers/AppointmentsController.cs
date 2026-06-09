using DoctorConnect.DbServices.IServices;
using DoctorConnect.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static DoctorConnect.Models.Enums;

namespace DoctorConnect.Controllers
{
    [Authorize]
    public class AppointmentsController : Controller
    {
        private readonly IAppointmentService _appointmentService;
        private readonly IDoctorService _doctorService;
        private readonly IPatientService _patientService;
        private readonly INotificationService _notificationService;

        public AppointmentsController(IAppointmentService appointmentService, IDoctorService doctorService, IPatientService patientService, INotificationService notificationService)
        {
            _appointmentService = appointmentService;
            _doctorService = doctorService;
            _patientService = patientService;
            _notificationService = notificationService;
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> Index()
        {
            var list = await _appointmentService.GetAllAsync();
            return View(list);
        }

        [Authorize(Roles = "Patient,Admin,SuperAdmin")]
        public async Task<IActionResult> Create()
        {
            ViewBag.Doctors = await _doctorService.GetAllAsync();
            ViewBag.Patients = await _patientService.GetAllAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Patient,Admin,SuperAdmin")]
        public async Task<IActionResult> Create(Appointment model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Doctors = await _doctorService.GetAllAsync();
                ViewBag.Patients = await _patientService.GetAllAsync();
                return View(model);
            }

            await _appointmentService.CreateAsync(model);

            // Create notification for doctor
            var docNotification = new Notification { UserId = model.Doctor?.User.Email ?? string.Empty, Message = $"New appointment booked for {model.AppointmentDate:d}", IsRead = false };
            await _notificationService.CreateAsync(docNotification);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(string id)
        {
            var a = await _appointmentService.GetByIdAsync(id);
            if (a == null) return NotFound();
            return View(a);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Appointment model)
        {
            if (!ModelState.IsValid) return View(model);
            await _appointmentService.UpdateAsync(model);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            await _appointmentService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Doctor,Admin,SuperAdmin")]
        public async Task<IActionResult> ChangeStatus(string id, AppointmentStatus status)
        {
            var a = await _appointmentService.GetByIdAsync(id);
            if (a == null) return NotFound();
            a.Status = status;
            await _appointmentService.UpdateAsync(a);

            // Notify patient
            var notif = new Notification { UserId = a.Patient?.User.Email ?? string.Empty, Message = $"Your appointment on {a.AppointmentDate:d} is {status}", IsRead = false };
            await _notificationService.CreateAsync(notif);

            return RedirectToAction(nameof(Index));
        }
    }
}
