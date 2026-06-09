using DoctorConnect.DbServices.IServices;
using DoctorConnect.Models;
using Microsoft.AspNetCore.Mvc;

namespace DoctorConnect.Controllers
{
    public class ClinicsController : Controller
    {
        private readonly IClinicService _clinicService;
        public ClinicsController(IClinicService clinicService)
        {
            _clinicService = clinicService;
        }
        public async Task<IActionResult> Index()
        {
            var clinics = await _clinicService.GetAllAsync();
            return View(clinics);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Clinic model)
        {
            if (!ModelState.IsValid) RedirectToAction(nameof(Index));
            await _clinicService.CreateAsync(model);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Clinic model)
        {
            if (!ModelState.IsValid) RedirectToAction(nameof(Index));
            await _clinicService.UpdateAsync(model);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            await _clinicService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
