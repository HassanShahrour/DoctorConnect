using DoctorConnect.DbServices.IServices;
using DoctorConnect.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoctorConnect.Controllers
{
    [Authorize]
    public class MedicalRecordsController : Controller
    {
        private readonly IMedicalRecordService _recordService;
        private readonly IPatientService _patientService;
        private readonly IDoctorService _doctorService;

        public MedicalRecordsController(IMedicalRecordService recordService, IPatientService patientService, IDoctorService doctorService)
        {
            _recordService = recordService;
            _patientService = patientService;
            _doctorService = doctorService;
        }

        [Authorize(Roles = "Doctor,Admin,SuperAdmin")]
        public async Task<IActionResult> Index()
        {
            var list = await _recordService.GetAllAsync();
            return View(list);
        }

        [Authorize(Roles = "Doctor,Admin,SuperAdmin")]
        public async Task<IActionResult> Create()
        {
            ViewBag.Patients = await _patientService.GetAllAsync();
            ViewBag.Doctors = await _doctorService.GetAllAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Doctor,Admin,SuperAdmin")]
        public async Task<IActionResult> Create(MedicalRecord model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Patients = await _patientService.GetAllAsync();
                ViewBag.Doctors = await _doctorService.GetAllAsync();
                return View(model);
            }

            await _recordService.CreateAsync(model);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(string id)
        {
            var r = await _recordService.GetByIdAsync(id);
            if (r == null) return NotFound();
            return View(r);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(MedicalRecord model)
        {
            if (!ModelState.IsValid) return View(model);
            await _recordService.UpdateAsync(model);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Doctor,Admin,SuperAdmin")]
        public async Task<IActionResult> Delete(string id)
        {
            await _recordService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
