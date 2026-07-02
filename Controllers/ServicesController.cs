using DoctorConnect.DbServices.IServices;
using DoctorConnect.Models;
using DoctorConnect.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoctorConnect.Controllers
{
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class ServicesController : Controller
    {
        private readonly IServiceService _serviceService;
        private readonly IDoctorService _doctorService;
        private readonly IClinicService _clinicService;

        public ServicesController(IServiceService serviceService, IDoctorService doctorService, IClinicService clinicService)
        {
            _serviceService = serviceService;
            _doctorService = doctorService;
            _clinicService = clinicService;
        }

        public async Task<IActionResult> Doctor(string doctorId)
        {
            var doctor = await _doctorService.GetWithServicesAsync(doctorId);
            if (doctor == null)
            {
                return NotFound();
            }

            var doctorName = string.Join(" ", new[] { doctor.User?.FirstName, doctor.User?.LastName }
                .Where(x => !string.IsNullOrWhiteSpace(x)));

            var model = new ServiceManagementViewModel
            {
                OwnerId = doctor.Id,
                OwnerName = string.IsNullOrWhiteSpace(doctorName) ? "Doctor" : $"Dr. {doctorName}",
                OwnerType = "Doctor",
                Services = doctor.Services?.OrderBy(s => s.Name).ThenByDescending(s => s.CreatedAt) ?? Enumerable.Empty<Service>(),
                DoctorId = doctor.Id,
            };

            return View("Index", model);
        }

        public async Task<IActionResult> Clinic(string clinicId)
        {
            var clinic = await _clinicService.GetWithServicesAsync(clinicId);
            if (clinic == null)
            {
                return NotFound();
            }

            var model = new ServiceManagementViewModel
            {
                OwnerId = clinic.Id,
                OwnerName = clinic.Name,
                OwnerType = "Clinic",
                Services = clinic.Services?.OrderBy(s => s.Name).ThenByDescending(s => s.CreatedAt) ?? Enumerable.Empty<Service>(),
                ClinicId = clinic.Id,
            };

            return View("Index", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ServiceManagementViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Please provide valid service details.";
                return RedirectToOwner(model.DoctorId, model.ClinicId);
            }

            var service = new Service
            {
                Name = model.Name,
                Description = model.Description,
                Fees = model.Fees,
                IsActive = model.IsActive,
                DoctorId = model.DoctorId,
                ClinicId = model.ClinicId
            };

            if (!string.IsNullOrWhiteSpace(model.DoctorId))
            {
                await _serviceService.CreateForDoctorAsync(service);
                TempData["Success"] = "Doctor service added successfully.";
                return RedirectToAction(nameof(Doctor), new { doctorId = model.DoctorId });
            }

            await _serviceService.CreateForClinicAsync(service);
            TempData["Success"] = "Clinic service added successfully.";
            return RedirectToAction(nameof(Clinic), new { clinicId = model.ClinicId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ServiceManagementViewModel model)
        {
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(model.Id))
            {
                TempData["Error"] = "Please provide valid service details.";
                return RedirectToOwner(model.DoctorId, model.ClinicId);
            }

            var service = await _serviceService.GetByIdAsync(model.Id);
            if (service == null)
            {
                TempData["Error"] = "Service not found.";
                return RedirectToOwner(model.DoctorId, model.ClinicId);
            }

            service.Name = model.Name;
            service.Description = model.Description;
            service.Fees = model.Fees;
            service.IsActive = model.IsActive;

            await _serviceService.UpdateAsync(service);
            TempData["Success"] = "Service updated successfully.";
            return RedirectToOwner(service.DoctorId, service.ClinicId);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id, string? doctorId, string? clinicId)
        {
            await _serviceService.DeleteAsync(id);
            TempData["Success"] = "Service deleted successfully.";
            return RedirectToOwner(doctorId, clinicId);
        }

        private IActionResult RedirectToOwner(string? doctorId, string? clinicId)
        {
            if (!string.IsNullOrWhiteSpace(doctorId))
            {
                return RedirectToAction(nameof(Doctor), new { doctorId });
            }

            return RedirectToAction(nameof(Clinic), new { clinicId });
        }
    }
}
