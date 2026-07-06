using DoctorConnect.Authorization;
using DoctorConnect.DbServices.IServices;
using DoctorConnect.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoctorConnect.Controllers
{
    [Authorize]
    public class SettingsController : Controller
    {
        private readonly ISettingsService _settingsService;

        public SettingsController(ISettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        [Authorize(Policy = AppPermissions.Settings.Read)]
        public async Task<IActionResult> Index()
        {
            var settings = await _settingsService.GetAsync() ?? new Settings();
            return View(settings);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = AppPermissions.Settings.Update)]
        public async Task<IActionResult> Index([Bind("NumberOfDaysToDisplay,Id")] Settings model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await _settingsService.CreateOrUpdateAsync(model);
            TempData["Success"] = "Settings saved.";
            return RedirectToAction(nameof(Index));
        }
    }
}
