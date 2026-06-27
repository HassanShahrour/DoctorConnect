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
        private readonly IDoctorService _doctorService;

        public AvailabilitiesController(IAvailabilityService availabilityService, IDoctorService doctorService)
        {
            _availabilityService = availabilityService;
            _doctorService = doctorService;
        }

        public async Task<IActionResult> Availability(string doctorId)
        {
            var availabilities = await _availabilityService.GetByIdAsync(doctorId);
            var doctor = await _doctorService.GetByIdAsync(doctorId);
            var clinics = doctor.Clinics;
            var clinicOptions = clinics.Select(c => new SelectListItem { Value = c.Id, Text = c.Name }).ToList();
            var model = new DoctorAvailabilityViewModel
            {
                DoctorId = doctorId,
                Clinics = clinics.Select(c => new ClinicHeaderViewModel
                {
                    ClinicId = c.Id,
                    ClinicName = c.Name,
                    ClinicOptions = clinicOptions
                }).ToList(),
                Days = Enum.GetValues<DayOfWeek>().Select(day => new DoctorAvailabilityDayRowViewModel
                {
                    DayOfWeek = day,
                    PerClinicAvailabilities = clinics.Select(clinic =>
                    {
                        var availability = availabilities?.FirstOrDefault(a => a.DayOfWeek == day && a.ClinicId == clinic.Id);
                        return new DoctorAvailabilityCellViewModel
                        {
                            ClinicId = clinic.Id,
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
                    }).ToList()
                }).ToList()
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SaveAvailability(DoctorAvailabilityViewModel model)
        {
            if (!ModelState.IsValid)
                return RedirectToAction(nameof(Availability), new { doctorId = model.DoctorId });
            var headerClinicIds = model.Clinics.Select(c => c.ClinicId).ToList();
            var availabilities = new List<DoctorAvailability>();
            for (int dayIndex = 0; dayIndex < model.Days.Count; dayIndex++)
            {
                var dayRow = model.Days[dayIndex];
                for (int col = 0; col < dayRow.PerClinicAvailabilities.Count; col++)
                {
                    var cell = dayRow.PerClinicAvailabilities[col];
                    var clinicId = headerClinicIds.ElementAtOrDefault(col) ?? cell.ClinicId;
                    if (string.IsNullOrEmpty(clinicId) || !cell.IsAvailable || cell.StartTime == null || cell.EndTime == null)
                        continue;
                    var entry = new DoctorAvailability
                    {
                        DoctorId = model.DoctorId,
                        ClinicId = clinicId,
                        DayOfWeek = dayRow.DayOfWeek,
                        StartTime = cell.StartTime ?? TimeSpan.Zero,
                        EndTime = cell.EndTime ?? TimeSpan.Zero,
                        DurationInMinutes = cell.DurationInMinutes,
                        IsAvailable = cell.IsAvailable,
                        BreakTimes = cell.BreakTimes?.Select(b => new BreakTime
                        {
                            Start = b.Start ?? TimeSpan.Zero,
                            End = b.End ?? TimeSpan.Zero
                        }).ToList() ?? new List<BreakTime>()
                    };
                    availabilities.Add(entry);
                }
            }
            await _availabilityService.SaveAvailabilityAsync(model.DoctorId, availabilities);
            TempData["Success"] = "Availability saved successfully.";
            return RedirectToAction(nameof(Availability), new { doctorId = model.DoctorId });
        }
    }
}
