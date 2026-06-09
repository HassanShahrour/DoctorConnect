using DoctorConnect.DbServices.IServices;
using DoctorConnect.Models;
using DoctorConnect.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoctorConnect.Controllers
{
    [Authorize(Roles = "Doctor,Admin,SuperAdmin")]
    public class AvailabilitiesController : Controller
    {
        private readonly IAvailabilityService _availabilityService;

        public AvailabilitiesController(IAvailabilityService availabilityService)
        {
            _availabilityService = availabilityService;
        }

        public async Task<IActionResult> Availability(string doctorId)
        {
            var availabilities = await _availabilityService.GetByIdAsync(doctorId);

            var model = new DoctorAvailabilityViewModel
            {
                DoctorId = doctorId,
                Days = Enum.GetValues<DayOfWeek>()
                    .Select(day =>
                    {
                        var availability = availabilities?.FirstOrDefault(a => a.DayOfWeek == day);

                        return new DoctorAvailabilityDayViewModel
                        {
                            DayOfWeek = day,
                            IsAvailable = availability?.IsAvailable ?? false,
                            StartTime = availability?.StartTime,
                            EndTime = availability?.EndTime,
                            DurationInMinutes = availability?.DurationInMinutes ?? 30
                        };
                    })
                    .ToList()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SaveAvailability(DoctorAvailabilityViewModel model)
        {
            if (!ModelState.IsValid)
                return RedirectToAction(nameof(Availability), new { doctorId = model.DoctorId });

            var availabilities = model.Days.Select(day => new DoctorAvailability
            {
                DoctorId = model.DoctorId,
                DayOfWeek = day.DayOfWeek,
                StartTime = day.StartTime ?? TimeSpan.Zero,
                EndTime = day.EndTime ?? TimeSpan.Zero,
                DurationInMinutes = day.DurationInMinutes,
                IsAvailable = day.IsAvailable
            });

            await _availabilityService.SaveAvailabilityAsync(model.DoctorId, availabilities);

            TempData["Success"] = "Availability saved successfully.";

            return RedirectToAction(nameof(Availability), new { doctorId = model.DoctorId });
        }
    }
}
