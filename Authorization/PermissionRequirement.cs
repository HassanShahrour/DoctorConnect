using Microsoft.AspNetCore.Authorization;

namespace DoctorConnect.Authorization
{
    public class PermissionRequirement(string permission) : IAuthorizationRequirement
    {
        public string Permission { get; } = permission;
    }
}
