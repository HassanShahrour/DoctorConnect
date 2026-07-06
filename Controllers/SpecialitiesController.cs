using DoctorConnect.Authorization;
using DoctorConnect.DbServices.IServices;
using DoctorConnect.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoctorConnect.Controllers
{
    [Authorize]
    public class SpecialitiesController : Controller
    {
        private readonly ISpecialityService _specialityService;
        public SpecialitiesController(ISpecialityService specialityService)
        {
            _specialityService = specialityService;
        }

        [Authorize(Policy = AppPermissions.Specialities.Read)]
        public async Task<IActionResult> Index()
        {
            try
            {
                var specialties = await _specialityService.GetAllAsync();
                return View(specialties);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SpecialitiesController.Index: {ex}");
                TempData["Error"] = $"An error occurred loading specialties: {ex.Message}";
                return View(Enumerable.Empty<Specialty>());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = AppPermissions.Specialities.Create)]
        public async Task<IActionResult> Create(Specialty model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    TempData["Error"] = "Model validation failed. Please check your data.";
                    return RedirectToAction(nameof(Index));
                }
                await _specialityService.CreateAsync(model);
                TempData["Success"] = "Specialty created successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SpecialitiesController.Create: {ex}");
                TempData["Error"] = $"An error occurred creating specialty: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = AppPermissions.Specialities.Update)]
        public async Task<IActionResult> Edit(Specialty model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    TempData["Error"] = "Model validation failed. Please check your data.";
                    return RedirectToAction(nameof(Index));
                }
                await _specialityService.UpdateAsync(model);
                TempData["Success"] = "Specialty updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SpecialitiesController.Edit: {ex}");
                TempData["Error"] = $"An error occurred updating specialty: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = AppPermissions.Specialities.Delete)]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                await _specialityService.DeleteAsync(id);
                TempData["Success"] = "Specialty deleted successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SpecialitiesController.Delete: {ex}");
                TempData["Error"] = $"An error occurred deleting specialty: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
