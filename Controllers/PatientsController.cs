using DoctorConnect.Authorization;
using DoctorConnect.DbServices.IServices;
using DoctorConnect.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoctorConnect.Controllers
{
    [Authorize]
    public class PatientsController : Controller
    {
        private readonly IPatientService _patientService;

        public PatientsController(IPatientService patientService)
        {
            _patientService = patientService;
        }

        [Authorize(Policy = AppPermissions.Patients.Read)]
        public async Task<IActionResult> Index()
        {
            try
            {
                var patients = await _patientService.GetAllAsync();
                return View(patients);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in PatientsController.Index: {ex}");
                TempData["Error"] = $"An error occurred loading patients: {ex.Message}";
                return View(Enumerable.Empty<Patient>());
            }
        }

        [Authorize(Policy = AppPermissions.Patients.Create)]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = AppPermissions.Patients.Create)]
        public async Task<IActionResult> Create(Patient model)
        {
            try
            {
                if (!ModelState.IsValid) return View(model);
                await _patientService.CreateAsync(model);
                TempData["Success"] = "Patient profile created successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in PatientsController.Create: {ex}");
                TempData["Error"] = $"An error occurred creating patient: {ex.Message}";
                return View(model);
            }
        }

        [Authorize(Policy = AppPermissions.Patients.Update)]
        public async Task<IActionResult> Edit(string id)
        {
            try
            {
                var patient = await _patientService.GetByIdAsync(id);
                if (patient == null) return NotFound();
                return View(patient);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in PatientsController.Edit [GET]: {ex}");
                TempData["Error"] = $"An error occurred: {ex.Message}";
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = AppPermissions.Patients.Update)]
        public async Task<IActionResult> Edit(Patient model)
        {
            try
            {
                if (!ModelState.IsValid) return View(model);
                await _patientService.UpdateAsync(model);
                TempData["Success"] = "Patient profile updated successfully.";
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in PatientsController.Edit [POST]: {ex}");
                TempData["Error"] = $"An error occurred: {ex.Message}";
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = AppPermissions.Patients.Delete)]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                await _patientService.DeleteAsync(id);
                TempData["Success"] = "Patient profile deleted successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in PatientsController.Delete: {ex}");
                TempData["Error"] = $"An error occurred: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
