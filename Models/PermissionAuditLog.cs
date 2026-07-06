namespace DoctorConnect.Models
{
    public class PermissionAuditLog
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string RoleId { get; set; } = string.Empty;
        public string PermissionId { get; set; } = string.Empty;
        public bool OldValue { get; set; }
        public bool NewValue { get; set; }
        public string? Scope { get; set; }
        public string? ChangedByUserId { get; set; }
        public ApplicationUser? ChangedByUser { get; set; }
        public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
        public string? Reason { get; set; }
    }
}
