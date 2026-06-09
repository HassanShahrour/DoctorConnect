using DoctorConnect.DbServices.IServices;
using DoctorConnect.Models;
using Microsoft.AspNetCore.Mvc;

namespace DoctorConnect.Controllers
{
    public class SpecialitiesController : Controller
    {
        private readonly ISpecialityService _specialityService;
        public SpecialitiesController(ISpecialityService specialityService)
        {
            _specialityService = specialityService;
        }
        public async Task<IActionResult> Index()
        {
            var specialties = await _specialityService.GetAllAsync();
            return View(specialties);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Specialty model)
        {
            if (!ModelState.IsValid) RedirectToAction(nameof(Index));
            await _specialityService.CreateAsync(model);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Specialty model)
        {
            if (!ModelState.IsValid) RedirectToAction(nameof(Index));
            await _specialityService.UpdateAsync(model);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            await _specialityService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
