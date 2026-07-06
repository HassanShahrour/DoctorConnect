using DoctorConnect.DbServices.IServices;
using Microsoft.AspNetCore.Authorization;

namespace DoctorConnect.Authorization
{
    public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly IPermissionService _permissionService;

        public PermissionAuthorizationHandler(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            if (context.User?.Identity?.IsAuthenticated != true)
            {
                return;
            }

            if (context.User.HasClaim(PermissionClaimTypes.Permission, requirement.Permission))
            {
                context.Succeed(requirement);
                return;
            }

            var userId = context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userId))
            {
                return;
            }

            var permissions = await _permissionService.GetUserPermissionsAsync(userId);
            if (permissions.Contains(requirement.Permission, StringComparer.OrdinalIgnoreCase))
            {
                context.Succeed(requirement);
            }
        }
    }
}
