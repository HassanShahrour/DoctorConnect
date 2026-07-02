using DoctorConnect.DbServices.IServices;
using DoctorConnect.Models;
using DoctorConnect.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DoctorConnect.Controllers
{
    [Authorize(Roles = "Doctor,Admin,SuperAdmin")]
    public class AppointmentsController : Controller
    {
        private readonly IAppointmentService _appointmentService;
        private readonly IDoctorService _doctorService;
        private readonly IPatientService _patientService;
        private readonly ISettingsService _settingsService;

        public AppointmentsController(
            IAppointmentService appointmentService,
            IDoctorService doctorService,
            IPatientService patientService,
            ISettingsService settingsService)
        {
            _appointmentService = appointmentService;
            _doctorService = doctorService;
            _patientService = patientService;
            _settingsService = settingsService;
        }

        public async Task<IActionResult> Index(string doctorId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(doctorId)) return BadRequest();
                var doctor = await _doctorService.GetByIdAsync(doctorId);
                if (doctor == null) return NotFound();

                var appointments = (doctor.Appointments ?? new List<Appointment>())
                    .OrderBy(a => a.AppointmentDate)
                    .ThenBy(a => a.AppointmentTime)
                    .ToList();

                var model = new AppointmentViewModel
                {
                    DoctorId = doctorId,
                    DoctorName = string.Join(" ", new[] { doctor.User?.FirstName, doctor.User?.LastName }.Where(x => !string.IsNullOrWhiteSpace(x))),
                    Appointments = appointments
                };
                return View(model);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in AppointmentsController.Index: {ex}");
                TempData["Error"] = $"An error occurred loading appointments: {ex.Message}";
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Calendar(string doctorId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(doctorId)) return BadRequest();
                var doctor = await _doctorService.GetByIdAsync(doctorId);
                if (doctor == null) return NotFound();

                var appointments = (doctor.Appointments ?? new List<Appointment>())
                    .OrderBy(a => a.AppointmentDate)
                    .ThenBy(a => a.AppointmentTime)
                    .ToList();

                var model = new AppointmentViewModel
                {
                    DoctorId = doctorId,
                    DoctorName = string.Join(" ", new[] { doctor.User?.FirstName, doctor.User?.LastName }.Where(x => !string.IsNullOrWhiteSpace(x))),
                    Appointments = appointments,
                    CalendarDays = appointments
                        .GroupBy(a => a.AppointmentDate.Date)
                        .OrderBy(g => g.Key)
                        .Select(g => new AppointmentCalendarDayViewModel
                        {
                            Date = g.Key,
                            AppointmentCount = g.Count(),
                            Appointments = g
                                .OrderBy(a => a.AppointmentTime)
                                .Select(a => new AppointmentCalendarItemViewModel
                                {
                                    Id = a.Id,
                                    PatientName = a.IsGuestPatient
                                        ? (a.GuestPatientFullName ?? "Guest patient")
                                        : string.Join(" ", new[] { a.Patient?.User?.FirstName, a.Patient?.User?.LastName }.Where(x => !string.IsNullOrWhiteSpace(x))),
                                    Notes = a.Notes,
                                    Status = a.Status.ToString(),
                                    TimeDisplay = DateTime.Today.Add(a.AppointmentTime).ToString("hh:mm tt"),
                                    Fees = a.Fees,
                                    IsGuestPatient = a.IsGuestPatient
                                })
                                .ToList()
                        })
                        .ToList()
                };

                return View(model);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in AppointmentsController.Calendar: {ex}");
                TempData["Error"] = $"An error occurred loading calendar: {ex.Message}";
                return RedirectToAction(nameof(Index), new { doctorId });
            }
        }

        public async Task<IActionResult> Create(string id)
        {
            try
            {
                var doctor = await _doctorService.GetByIdAsync(id);
                if (doctor == null) return NotFound();

                var clinics = doctor.Clinics ?? new List<Clinic>();
                var settings = await _settingsService.GetAsync();
                var daysToShow = settings?.NumberOfDaysToDisplay ?? 14;

                var model = new CreateAppointmentViewModel
                {
                    DoctorId = id,
                    Clinics = clinics.Select(c => new SelectListItem { Value = c.Id, Text = c.Name }).ToList(),
                    SelectedClinicId = clinics.Count() == 1 ? clinics.First().Id : null,
                    IsGuestPatient = false,
                    AppointmentDate = DateTime.Today
                };
                ViewData["DaysToShow"] = daysToShow;
                return View(model);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in AppointmentsController.Create [GET]: {ex}");
                TempData["Error"] = $"An error occurred: {ex.Message}";
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateAppointmentViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var doctor = await _doctorService.GetByIdAsync(model.DoctorId);
                    model.Clinics = doctor.Clinics?.Select(c => new SelectListItem { Value = c.Id, Text = c.Name }).ToList() ?? new List<SelectListItem>();
                    TempData["Error"] = "Please correct the model information.";
                    return View(model);
                }

                var appointment = new Appointment
                {
                    DoctorId = model.DoctorId,
                    PatientId = model.IsGuestPatient ? null : model.PatientId,
                    ClinicId = model.SelectedClinicId,
                    AppointmentDate = model.AppointmentDate.Date,
                    AppointmentTime = model.AppointmentTime,
                    Notes = model.Notes,
                    Fees = model.Fees,
                    Status = AppointmentStatus.Confirmed,
                    IsGuestPatient = model.IsGuestPatient,
                    GuestPatientFullName = model.IsGuestPatient ? model.GuestPatientFullName : null,
                    GuestPatientPhoneNumber = model.IsGuestPatient ? model.GuestPatientPhoneNumber : null,
                    GuestPatientGender = model.IsGuestPatient ? model.GuestPatientGender : null,
                    GuestPatientDateOfBirth = model.IsGuestPatient ? model.GuestPatientDateOfBirth : null
                };

                await _appointmentService.CreateAsync(appointment);
                TempData["Success"] = "Appointment created successfully.";
                return RedirectToAction(nameof(Index), new { doctorId = model.DoctorId });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in AppointmentsController.Create [POST]: {ex}");
                TempData["Error"] = $"An error occurred creating appointment: {ex.Message}";
                var doctor = await _doctorService.GetByIdAsync(model.DoctorId);
                model.Clinics = doctor?.Clinics?.Select(c => new SelectListItem { Value = c.Id, Text = c.Name }).ToList() ?? new List<SelectListItem>();
                return View(model);
            }
        }

        public async Task<IActionResult> Edit(string id)
        {
            try
            {
                var appt = await _appointmentService.GetByIdAsync(id);
                if (appt == null) return NotFound();
                var model = CreateAppointmentViewModel.FromAppointment(appt);
                var doctor = await _doctorService.GetByIdAsync(appt.DoctorId ?? string.Empty);
                model.Clinics = doctor.Clinics?.Select(c => new SelectListItem { Value = c.Id, Text = c.Name }).ToList() ?? new List<SelectListItem>();
                model.SelectedClinicId = appt.ClinicId;
                var settings = await _settingsService.GetAsync();
                ViewData["DaysToShow"] = settings?.NumberOfDaysToDisplay;
                return View(model);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in AppointmentsController.Edit [GET]: {ex}");
                TempData["Error"] = $"An error occurred loaded edit page: {ex.Message}";
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CreateAppointmentViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var doctor = await _doctorService.GetByIdAsync(model.DoctorId);
                    model.Clinics = doctor.Clinics?.Select(c => new SelectListItem { Value = c.Id, Text = c.Name }).ToList() ?? new List<SelectListItem>();
                    TempData["Error"] = "Please check input fields.";
                    return View(model);
                }
                var appt = await _appointmentService.GetByIdAsync(model.Id);
                if (appt == null) return NotFound();
                appt.ClinicId = model.SelectedClinicId;
                appt.AppointmentDate = model.AppointmentDate.Date;
                appt.AppointmentTime = model.AppointmentTime;
                appt.Notes = model.Notes;
                appt.Fees = model.Fees;
                appt.Status = model.Status;
                appt.IsGuestPatient = model.IsGuestPatient;
                if (model.IsGuestPatient)
                {
                    appt.PatientId = null;
                    appt.GuestPatientFullName = model.GuestPatientFullName;
                    appt.GuestPatientPhoneNumber = model.GuestPatientPhoneNumber;
                    appt.GuestPatientGender = model.GuestPatientGender;
                    appt.GuestPatientDateOfBirth = model.GuestPatientDateOfBirth;
                }
                else
                {
                    appt.PatientId = model.PatientId;
                    appt.GuestPatientFullName = null;
                    appt.GuestPatientPhoneNumber = null;
                    appt.GuestPatientGender = null;
                    appt.GuestPatientDateOfBirth = null;
                }
                await _appointmentService.UpdateAsync(appt);
                TempData["Success"] = "Appointment updated.";
                return RedirectToAction(nameof(Index), new { doctorId = model.DoctorId });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in AppointmentsController.Edit [POST]: {ex}");
                TempData["Error"] = $"An error occurred updating appointment: {ex.Message}";
                var doctor = await _doctorService.GetByIdAsync(model.DoctorId);
                model.Clinics = doctor?.Clinics?.Select(c => new SelectListItem { Value = c.Id, Text = c.Name }).ToList() ?? new List<SelectListItem>();
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var appt = await _appointmentService.GetByIdAsync(id);
                if (appt == null) return NotFound();
                await _appointmentService.DeleteAsync(id);
                TempData["Success"] = "Appointment deleted.";
                return RedirectToAction(nameof(Index), new { doctorId = appt.DoctorId });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in AppointmentsController.Delete: {ex}");
                TempData["Error"] = $"An error occurred deleting appointment: {ex.Message}";
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Patients(string q)
        {
            try
            {
                var all = await _patientService.GetAllAsync();
                var filtered = all.Where(p => string.IsNullOrWhiteSpace(q) ||
                    (p.User != null && ((p.User.FirstName + " " + p.User.LastName).Contains(q, StringComparison.OrdinalIgnoreCase))) ||
                    (p.User != null && (p.User.PhoneNumber ?? "").Contains(q, StringComparison.OrdinalIgnoreCase))
                ).Select(p => new { id = p.Id, text = p.User == null ? "" : (p.User.FirstName + " " + p.User.LastName + " - " + p.User.Email) }).ToList();
                return Ok(filtered);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in AppointmentsController.Patients: {ex}");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Slots(string doctorId, string clinicId, DateTime date)
        {
            try
            {
                var doctor = await _doctorService.GetByIdAsync(doctorId);
                if (doctor == null) return NotFound();
                var availability = doctor.Availabilities?.FirstOrDefault(a => a.ClinicId == clinicId && a.DayOfWeek == date.DayOfWeek && a.IsAvailable);
                if (availability == null) return Ok(new { slots = new List<object>() });
                var duration = availability.DurationInMinutes > 0 ? availability.DurationInMinutes : 30;
                var start = date.Date + availability.StartTime;
                var end = date.Date + availability.EndTime;
                var existing = (await _appointmentService.GetByDoctorInRangeAsync(doctorId, date.Date, date.Date)).Where(a => a.Status != AppointmentStatus.Cancelled).ToList();
                var slots = new List<object>();
                for (var t = start; t.AddMinutes(duration) <= end; t = t.AddMinutes(duration))
                {
                    var slotStart = t;
                    var slotEnd = t.AddMinutes(duration);
                    var overlapping = existing.Any(a => (date.Date + a.AppointmentTime) < slotEnd && ((date.Date + a.AppointmentTime).AddMinutes(duration)) > slotStart);
                    slots.Add(new { time = slotStart.TimeOfDay, available = !overlapping });
                }
                return Ok(new { slots });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in AppointmentsController.Slots: {ex}");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> AvailableDates(string doctorId, string clinicId)
        {
            try
            {
                var doctor = await _doctorService.GetByIdAsync(doctorId);
                if (doctor == null) return NotFound();
                var settings = await _settingsService.GetAsync();
                var today = DateTime.Today;
                var dates = new List<object>();
                for (int i = 0; i < (settings?.NumberOfDaysToDisplay ?? 14); i++)
                {
                    var d = today.AddDays(i);
                    var availability = doctor.Availabilities?.FirstOrDefault(a => a.ClinicId == clinicId && a.DayOfWeek == d.DayOfWeek && a.IsAvailable);
                    if (availability == null) continue;
                    var duration = availability.DurationInMinutes > 0 ? availability.DurationInMinutes : 30;
                    var start = d.Date + availability.StartTime;
                    var end = d.Date + availability.EndTime;
                    var existing = (await _appointmentService.GetByDoctorInRangeAsync(doctorId, d.Date, d.Date)).Where(a => a.Status != AppointmentStatus.Cancelled).ToList();
                    var hasAvailable = false;
                    for (var t = start; t.AddMinutes(duration) <= end; t = t.AddMinutes(duration))
                    {
                        var slotStart = t;
                        var slotEnd = t.AddMinutes(duration);
                        var overlapping = existing.Any(a => (d.Date + a.AppointmentTime) < slotEnd && ((d.Date + a.AppointmentTime).AddMinutes(duration)) > slotStart);
                        if (!overlapping)
                        {
                            hasAvailable = true; break;
                        }
                    }
                    if (hasAvailable)
                    {
                        dates.Add(new { date = d.ToString("yyyy-MM-dd"), day = d.ToString("dddd"), dayNumber = d.Day });
                    }
                }
                return Ok(dates);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in AppointmentsController.AvailableDates: {ex}");
                return StatusCode(500, ex.Message);
            }
        }
    }
}
