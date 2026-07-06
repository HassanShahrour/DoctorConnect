using DoctorConnect.Authorization;
using DoctorConnect.DbServices.IServices;
using DoctorConnect.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoctorConnect.Controllers
{
    [Authorize]
    public class ClinicsController : Controller
    {
        private readonly IClinicService _clinicService;
        public ClinicsController(IClinicService clinicService)
        {
            _clinicService = clinicService;
        }

        [Authorize(Policy = AppPermissions.Clinics.Read)]
        public async Task<IActionResult> Index()
        {
            try
            {
                var clinics = await _clinicService.GetAllAsync();
                return View(clinics);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in ClinicsController.Index: {ex}");
                TempData["Error"] = $"An error occurred loading clinics: {ex.Message}";
                return View(Enumerable.Empty<Clinic>());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = AppPermissions.Clinics.Create)]
        public async Task<IActionResult> Create(Clinic model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    TempData["Error"] = "Invalid clinic details supplied.";
                    return RedirectToAction(nameof(Index));
                }
                await _clinicService.CreateAsync(model);
                TempData["Success"] = "Clinic created successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in ClinicsController.Create: {ex}");
                TempData["Error"] = $"An error occurred creating clinic: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = AppPermissions.Clinics.Update)]
        public async Task<IActionResult> Edit(Clinic model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    TempData["Error"] = "Invalid clinic details supplied.";
                    return RedirectToAction(nameof(Index));
                }
                await _clinicService.UpdateAsync(model);
                TempData["Success"] = "Clinic updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in ClinicsController.Edit: {ex}");
                TempData["Error"] = $"An error occurred updating clinic: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = AppPermissions.Clinics.Delete)]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                await _clinicService.DeleteAsync(id);
                TempData["Success"] = "Clinic deleted successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in ClinicsController.Delete: {ex}");
                TempData["Error"] = $"An error occurred deleting clinic: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
