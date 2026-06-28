using DoctorConnect.DbServices.IServices;
using DoctorConnect.DTOs;
using DoctorConnect.Models;
using Microsoft.AspNetCore.Mvc;

namespace DoctorConnect.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class DoctorApiController : ControllerBase
    {
        private readonly IDoctorService _doctorService;
        private readonly IAppointmentService _appointmentService;
        private readonly ISettingsService _settingsService;

        public DoctorApiController(IDoctorService doctorService, IAppointmentService appointmentService, ISettingsService settingsService)
        {
            _doctorService = doctorService;
            _appointmentService = appointmentService;
            _settingsService = settingsService;
        }

        // GET: api/DoctorApi
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var doctors = await _doctorService.GetAllAsync();
            var result = doctors.Where(d => d.IsActive).Select(d => new
            {
                id = d.Id,
                firstName = d.User?.FirstName,
                lastName = d.User?.LastName,
                fullName = string.Join(' ', new[] { d.User?.FirstName, d.User?.LastName }.Where(s => !string.IsNullOrWhiteSpace(s))),
                specialty = d.Specialty == null ? null : new { id = d.Specialty.Id, name = d.Specialty.Name },
                clinics = d.Clinics?.Select(c => new { id = c.Id, name = c.Name, address = c.Address }).ToList(),
                profilePhoto = d.ProfilePhoto,
                consultationFee = d.ConsultationFee,
                yearsOfExperience = d.YearsOfExperience,
                biography = d.Biography,
                isActive = d.IsActive,
                address = d.User?.Address,
            });
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Doctors fetched successfully.",
                Data = result
            });
        }

        // GET: api/DoctorApi/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Invalid id.",
                    Data = null
                });
            var d = await _doctorService.GetByIdAsync(id);
            if (d == null)
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Doctor not found.",
                    Data = null
                });
            var today = DateTime.Today;
            var settings = await _settingsService.GetAsync();
            var rangeEnd = today.AddDays(settings.NumberOfDaysToDisplay);
            var existingAppointments = (await _appointmentService.GetByDoctorInRangeAsync(id, today, rangeEnd))
                .Where(a => a.Status != AppointmentStatus.Cancelled)
                .ToList();
            var availabilitiesByClinic = (d.Availabilities ?? new List<DoctorAvailability>())
                .Where(a => a.IsAvailable)
                .GroupBy(a => a.ClinicId)
                .Select(clinicGroup => new
                {
                    clinicId = clinicGroup.Key,
                    slots = Enumerable.Range(0, settings.NumberOfDaysToDisplay)
                        .Select(offset => today.AddDays(offset))
                        .SelectMany(date => clinicGroup
                            .Where(a => a.DayOfWeek == date.DayOfWeek)
                            .SelectMany(availability => BuildAvailableSlots(date, availability, existingAppointments)))
                        .OrderBy(slot => slot.Date)
                        .ThenBy(slot => slot.Time)
                        .Select(slot => new
                        {
                            date = slot.Date,
                            time = slot.Time
                        })
                        .ToList()
                })
                .Where(x => x.slots.Any())
                .ToList();
            var doctor = new
            {
                id = d.Id,
                fullName = string.Join(' ', new[] { d.User?.FirstName, d.User?.LastName }.Where(s => !string.IsNullOrWhiteSpace(s))),
                specialty = d.Specialty == null ? null : new { id = d.Specialty.Id, name = d.Specialty.Name },
                clinics = d.Clinics?.Select(c => new { id = c.Id, name = c.Name }).ToList(),
                consultationFee = d.ConsultationFee,
                profilePhoto = d.ProfilePhoto,
                availableClinicSlots = availabilitiesByClinic
            };
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Doctor fetched successfully.",
                Data = doctor
            });
        }

        private static IEnumerable<AvailableSlotDto> BuildAvailableSlots(DateTime date, DoctorAvailability availability, List<Appointment> existingAppointments)
        {
            var duration = availability.DurationInMinutes > 0 ? availability.DurationInMinutes : 30;
            var start = date.Date + availability.StartTime;
            var end = date.Date + availability.EndTime;
            var breakTimes = availability.BreakTimes ?? new List<BreakTime>();
            for (var slotStart = start; slotStart.AddMinutes(duration) <= end; slotStart = slotStart.AddMinutes(duration))
            {
                var slotEnd = slotStart.AddMinutes(duration);
                var isInBreak = breakTimes.Any(b =>
                {
                    var breakStart = date.Date + b.Start;
                    var breakEnd = date.Date + b.End;
                    return breakStart < slotEnd && slotStart < breakEnd;
                });
                if (isInBreak)
                {
                    continue;
                }
                var isBooked = existingAppointments.Any(a =>
                {
                    var appointmentStart = a.AppointmentDate.Date + a.AppointmentTime;
                    var appointmentEnd = appointmentStart.AddMinutes(duration);
                    return appointmentStart < slotEnd && slotStart < appointmentEnd;
                });
                if (!isBooked)
                {
                    yield return new AvailableSlotDto
                    {
                        Date = date.ToString("yyyy-MM-dd"),
                        Time = slotStart.ToString("HH:mm")
                    };
                }
            }
        }
    }
}
