using DoctorConnect.Authorization;
using DoctorConnect.DbServices.IServices;
using DoctorConnect.Models;
using DoctorConnect.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DoctorConnect.Controllers
{
    [Authorize]
    public class TasksController : Controller
    {
        private readonly IDoctorTaskService _doctorTaskService;
        private readonly IDoctorService _doctorService;

        public TasksController(IDoctorTaskService doctorTaskService, IDoctorService doctorService)
        {
            _doctorTaskService = doctorTaskService;
            _doctorService = doctorService;
        }

        [Authorize(Policy = AppPermissions.Tasks.Read)]
        public async Task<IActionResult> Index()
        {
            var doctor = await GetCurrentDoctorAsync();
            if (doctor == null)
            {
                TempData["Error"] = "Doctor profile was not found.";
                return RedirectToAction("Index", "Home");
            }

            var tasks = await _doctorTaskService.GetByDoctorIdAsync(doctor.Id);
            var doctorName = string.Join(" ", new[] { doctor.User?.FirstName, doctor.User?.LastName }
                .Where(x => !string.IsNullOrWhiteSpace(x)));

            var model = new DoctorTaskManagementViewModel
            {
                DoctorId = doctor.Id,
                DoctorName = string.IsNullOrWhiteSpace(doctorName) ? "Doctor" : $"Dr. {doctorName}",
                Tasks = tasks,
                TaskDate = DateTime.Today,
                Bullets = new List<TaskBulletInputViewModel>()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = AppPermissions.Tasks.Create)]
        public async Task<IActionResult> Create(DoctorTaskManagementViewModel model)
        {
            var doctor = await GetCurrentDoctorAsync();
            if (doctor == null)
            {
                TempData["Error"] = "Doctor profile was not found.";
                return RedirectToAction(nameof(Index));
            }

            model.DoctorId = doctor.Id;
            model.Bullets = NormalizeBullets(model.Bullets);

            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Please provide valid task details.";
                return RedirectToAction(nameof(Index));
            }

            await _doctorTaskService.CreateAsync(model);
            TempData["Success"] = "Task created successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = AppPermissions.Tasks.Update)]
        public async Task<IActionResult> Edit(DoctorTaskManagementViewModel model)
        {
            var doctor = await GetCurrentDoctorAsync();
            if (doctor == null)
            {
                TempData["Error"] = "Doctor profile was not found.";
                return RedirectToAction(nameof(Index));
            }

            model.DoctorId = doctor.Id;
            model.Bullets = NormalizeBullets(model.Bullets);

            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(model.Id))
            {
                TempData["Error"] = "Please provide valid task details.";
                return RedirectToAction(nameof(Index));
            }

            var task = await _doctorTaskService.GetByIdAsync(model.Id);
            if (task == null || task.DoctorId != doctor.Id)
            {
                TempData["Error"] = "Task not found.";
                return RedirectToAction(nameof(Index));
            }

            await _doctorTaskService.UpdateAsync(model);
            TempData["Success"] = "Task updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = AppPermissions.Tasks.Update)]
        public async Task<IActionResult> UpdateProgress(string id, int progress)
        {
            var doctor = await GetCurrentDoctorAsync();
            if (doctor == null)
            {
                return BadRequest();
            }

            var task = await _doctorTaskService.GetByIdAsync(id);
            if (task == null || task.DoctorId != doctor.Id)
            {
                return NotFound();
            }

            await _doctorTaskService.UpdateProgressAsync(id, progress);
            return Ok();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = AppPermissions.Tasks.Update)]
        public async Task<IActionResult> Cancel(string id)
        {
            var doctor = await GetCurrentDoctorAsync();
            if (doctor == null)
            {
                TempData["Error"] = "Doctor profile was not found.";
                return RedirectToAction(nameof(Index));
            }

            var task = await _doctorTaskService.GetByIdAsync(id);
            if (task == null || task.DoctorId != doctor.Id)
            {
                TempData["Error"] = "Task not found.";
                return RedirectToAction(nameof(Index));
            }

            await _doctorTaskService.CancelAsync(id);
            TempData["Success"] = "Task canceled successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = AppPermissions.Tasks.Update)]
        public async Task<IActionResult> Uncancel(string id)
        {
            var doctor = await GetCurrentDoctorAsync();
            if (doctor == null)
            {
                TempData["Error"] = "Doctor profile was not found.";
                return RedirectToAction(nameof(Index));
            }

            var task = await _doctorTaskService.GetByIdAsync(id);
            if (task == null || task.DoctorId != doctor.Id)
            {
                TempData["Error"] = "Task not found.";
                return RedirectToAction(nameof(Index));
            }

            await _doctorTaskService.UncancelAsync(id);
            TempData["Success"] = "Task restored successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = AppPermissions.Tasks.Delete)]
        public async Task<IActionResult> Delete(string id)
        {
            var doctor = await GetCurrentDoctorAsync();
            if (doctor == null)
            {
                TempData["Error"] = "Doctor profile was not found.";
                return RedirectToAction(nameof(Index));
            }

            var task = await _doctorTaskService.GetByIdAsync(id);
            if (task == null || task.DoctorId != doctor.Id)
            {
                TempData["Error"] = "Task not found.";
                return RedirectToAction(nameof(Index));
            }

            await _doctorTaskService.DeleteAsync(id);
            TempData["Success"] = "Task deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        private async Task<Doctor?> GetCurrentDoctorAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return null;
            }

            return await _doctorService.GetByUserIdAsync(userId);
        }

        private static List<TaskBulletInputViewModel> NormalizeBullets(List<TaskBulletInputViewModel>? bullets)
        {
            return bullets?
                .Where(b => !string.IsNullOrWhiteSpace(b.Description))
                .ToList() ?? new List<TaskBulletInputViewModel>();
        }
    }
}
