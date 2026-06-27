using DoctorConnect.DbServices.IServices;
using DoctorConnect.Models;
using DoctorConnect.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DoctorConnect.Controllers
{
    [Authorize(Roles = "Doctor,Admin,SuperAdmin")]
    public class AvailabilitiesController : Controller
    {
        private readonly IAvailabilityService _availabilityService;
        private readonly IClinicService _clinicService;

        public AvailabilitiesController(IAvailabilityService availabilityService, IClinicService clinicService)
        {
            _availabilityService = availabilityService;
            _clinicService = clinicService;
        }

        public async Task<IActionResult> Availability(string doctorId)
        {
            var availabilities = await _availabilityService.GetByIdAsync(doctorId);
            var clinics = await _clinicService.GetAllAsync();

            var model = new DoctorAvailabilityViewModel
            {
                DoctorId = doctorId,
                Clinics = clinics.Select(clinic => new ClinicAvailabilityViewModel
                {
                    ClinicId = clinic.Id,
                    ClinicName = clinic.Name,
                    Days = Enum.GetValues<DayOfWeek>()
                        .Select(day =>
                        {
                            var availability = availabilities?.FirstOrDefault(a => a.DayOfWeek == day && a.ClinicId == clinic.Id);
                            return new DoctorAvailabilityDayViewModel
                            {
                                DayOfWeek = day,
                                IsAvailable = availability?.IsAvailable ?? false,
                                StartTime = availability?.StartTime,
                                EndTime = availability?.EndTime,
                                DurationInMinutes = availability?.DurationInMinutes ?? 30,
                                BreakTimes = availability?.BreakTimes?.Select(b => new BreakTimeViewModel
                                {
                                    Start = b.Start,
                                    End = b.End
                                }).ToList() ?? new List<BreakTimeViewModel>()
                            };
                        })
                        .ToList()
                }).ToList()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SaveAvailability(DoctorAvailabilityViewModel model)
        {
            if (!ModelState.IsValid)
                return RedirectToAction(nameof(Availability), new { doctorId = model.DoctorId });

            var availabilities = model.Clinics.SelectMany(clinic => clinic.Days.Select(day => new DoctorAvailability
            {
                DoctorId = model.DoctorId,
                ClinicId = clinic.ClinicId,
                DayOfWeek = day.DayOfWeek,
                StartTime = day.StartTime ?? TimeSpan.Zero,
                EndTime = day.EndTime ?? TimeSpan.Zero,
                DurationInMinutes = day.DurationInMinutes,
                IsAvailable = day.IsAvailable,
                BreakTimes = day.BreakTimes?.Select(b => new BreakTime
                {
                    Start = b.Start ?? TimeSpan.Zero,
                    End = b.End ?? TimeSpan.Zero
                }).ToList() ?? new List<BreakTime>()
            }));

            await _availabilityService.SaveAvailabilityAsync(model.DoctorId, availabilities);

            TempData["Success"] = "Availability saved successfully.";

            return RedirectToAction(nameof(Availability), new { doctorId = model.DoctorId });
        }
    }
}
