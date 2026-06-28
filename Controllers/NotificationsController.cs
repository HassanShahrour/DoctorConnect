using DoctorConnect.DbServices.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoctorConnect.Controllers
{
    [Authorize]
    public class NotificationsController : Controller
    {
        private readonly INotificationService _notificationService;

        public NotificationsController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var userId = User.Identity?.Name ?? string.Empty;
                var list = await _notificationService.GetForUserAsync(userId);
                return View(list);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in NotificationsController.Index: {ex}");
                TempData["Error"] = $"An error occurred loading notifications: {ex.Message}";
                return View(Enumerable.Empty<DoctorConnect.Models.Notification>());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAsRead(string id)
        {
            try
            {
                await _notificationService.MarkAsReadAsync(id);
                TempData["Success"] = "Notification marked as read.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in NotificationsController.MarkAsRead: {ex}");
                TempData["Error"] = $"An error occurred: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
