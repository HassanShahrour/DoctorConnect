using DoctorConnect.DbServices.IServices;
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

        public DoctorApiController(IDoctorService doctorService)
        {
            _doctorService = doctorService;
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
                availabilities = d.Availabilities == null ? null : d.Availabilities.Select(a => new
                {
                    dayOfWeek = a.DayOfWeek,
                    isAvailable = a.IsAvailable,
                    startTime = a.StartTime,
                    endTime = a.EndTime,
                    durationInMinutes = a.DurationInMinutes
                })
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
            var doctor = new
            {
                id = d.Id,
                firstName = d.User?.FirstName,
                lastName = d.User?.LastName,
                fullName = string.Join(' ', new[] { d.User?.FirstName, d.User?.LastName }.Where(s => !string.IsNullOrWhiteSpace(s))),
                email = d.User?.Email,
                phoneNumber = d.User?.PhoneNumber,
                gender = d.User?.Gender,
                dateOfBirth = d.User?.DateOfBirth,
                address = d.User?.Address,
                specialty = d.Specialty == null ? null : new { id = d.Specialty.Id, name = d.Specialty.Name },
                clinics = d.Clinics?.Select(c => new { id = c.Id, name = c.Name, address = c.Address }).ToList(),
                profilePhoto = d.ProfilePhoto,
                consultationFee = d.ConsultationFee,
                yearsOfExperience = d.YearsOfExperience,
                biography = d.Biography,
                isActive = d.IsActive,
                availabilities = d.Availabilities == null ? null : d.Availabilities.Select(a => new
                {
                    dayOfWeek = a.DayOfWeek,
                    isAvailable = a.IsAvailable,
                    startTime = a.StartTime,
                    endTime = a.EndTime,
                    durationInMinutes = a.DurationInMinutes
                })
            };
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Doctor fetched successfully.",
                Data = doctor
            });
        }
    }
}
