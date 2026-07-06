using DoctorConnect.Authorization;
using DoctorConnect.DbServices.IServices;
using DoctorConnect.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DoctorConnect.Controllers
{
    [Authorize]
    public class PermissionsController : Controller
    {
        private readonly IPermissionService _permissionService;
        private readonly IAuthorizationService _authorizationService;

        public PermissionsController(IPermissionService permissionService, IAuthorizationService authorizationService)
        {
            _permissionService = permissionService;
            _authorizationService = authorizationService;
        }

        public async Task<IActionResult> Index()
        {
            if (!await CanAccessReadAsync())
            {
                return Forbid();
            }

            var model = await _permissionService.GetRolePermissionsAsync();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(RolePermissionsUpdateRequest input)
        {
            if (!await CanAccessUpdateAsync())
            {
                return Forbid();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _permissionService.UpdateRolePermissionsAsync(input, userId);
            TempData["Success"] = "Role permissions updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> CanAccessReadAsync()
        {
            if (User.IsInRole("SuperAdmin"))
            {
                return true;
            }

            return (await _authorizationService.AuthorizeAsync(User, AppPermissions.RolePermissions.Read)).Succeeded
                || (await _authorizationService.AuthorizeAsync(User, AppPermissions.RolePermissions.Update)).Succeeded;
        }

        private async Task<bool> CanAccessUpdateAsync()
        {
            if (User.IsInRole("SuperAdmin"))
            {
                return true;
            }

            return (await _authorizationService.AuthorizeAsync(User, AppPermissions.RolePermissions.Update)).Succeeded;
        }
    }
}
