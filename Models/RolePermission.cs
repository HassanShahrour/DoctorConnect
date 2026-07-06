using Microsoft.AspNetCore.Identity;

namespace DoctorConnect.Models
{
    public class RolePermission
    {
        public string RoleId { get; set; } = string.Empty;
        public IdentityRole Role { get; set; } = null!;
        public string PermissionId { get; set; } = string.Empty;
        public Permission Permission { get; set; } = null!;
        public string Scope { get; set; } = PermissionScopes.Any;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? CreatedByUserId { get; set; }
        public ApplicationUser? CreatedByUser { get; set; }
    }
}
