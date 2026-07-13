using DoctorConnect.DbServices.IServices;
using DoctorConnect.Models;
using Microsoft.AspNetCore.Mvc;

namespace DoctorConnect.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class BookingApiController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;
        private readonly IDoctorService _doctorService;

        public BookingApiController(IAppointmentService appointmentService, IDoctorService doctorService)
        {
            _appointmentService = appointmentService;
            _doctorService = doctorService;
        }

        // POST: api/BookingApi/book
        [HttpPost("book")]
        public async Task<IActionResult> Book([FromBody] Appointment model)
        {
            var doctorId = model.DoctorId;
            if (string.IsNullOrWhiteSpace(doctorId))
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "DoctorId is required.",
                    Data = null
                });
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)),
                    Data = null
                });
            var doctor = await _doctorService.GetByIdAsync(doctorId);
            if (doctor == null)
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Doctor not found.",
                    Data = null
                });
            var requestedDate = model.AppointmentDate.Date;
            var requestedStart = requestedDate + model.AppointmentTime;
            var dayOfWeek = model.AppointmentDate.DayOfWeek;
            var availability = doctor.Availabilities?.FirstOrDefault(a => a.DayOfWeek == dayOfWeek && a.IsAvailable);
            if (availability == null)
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Doctor is not available on the requested day.",
                    Data = null
                });
            var durationMinutes = availability.DurationInMinutes > 0 ? availability.DurationInMinutes : 30;
            var requestedEnd = requestedStart.AddMinutes(durationMinutes);
            var availStart = requestedDate + (availability.StartTime);
            var availEnd = requestedDate + (availability.EndTime);
            if (requestedStart < availStart || requestedEnd > availEnd)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Requested time is outside doctor's availability window.",
                    Data = new { start = availability.StartTime, end = availability.EndTime, durationInMinutes = durationMinutes }
                });
            }
            var existing = (await _appointmentService.GetByDoctorInRangeAsync(doctorId, requestedDate, requestedDate)).ToList();
            foreach (var appt in existing.Where(a => a.Status != AppointmentStatus.Cancelled))
            {
                var apptStart = appt.AppointmentDate.Date + appt.AppointmentTime;
                var apptDuration = durationMinutes;
                var apptEnd = apptStart.AddMinutes(apptDuration);
                if (requestedStart < apptEnd && apptStart < requestedEnd)
                {
                    return Conflict(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Requested slot conflicts with an existing appointment.",
                        Data = new
                        {
                            id = appt.Id,
                            date = appt.AppointmentDate.Date,
                            time = appt.AppointmentTime,
                            durationInMinutes = apptDuration,
                            status = appt.Status
                        }
                    });
                }
            }
            var appointment = new Appointment
            {
                DoctorId = doctorId,
                PatientId = model.PatientId,
                ClinicId = model.ClinicId,
                AppointmentDate = requestedDate,
                AppointmentTime = model.AppointmentTime,
                Notes = model.Notes,
                Fees = model.Fees,
                Status = model.Status,
                IsGuestPatient = model.IsGuestPatient,
                GuestPatientFullName = model.IsGuestPatient ? model.GuestPatientFullName : null,
                GuestPatientPhoneNumber = model.IsGuestPatient ? model.GuestPatientPhoneNumber : null,
                GuestPatientGender = model.IsGuestPatient ? model.GuestPatientGender : null,
                GuestPatientDateOfBirth = model.IsGuestPatient ? model.GuestPatientDateOfBirth : null
            };
            await _appointmentService.CreateAsync(appointment);
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Appointment created.",
                Data = new { appointmentId = appointment.Id }
            });
        }
    }
}