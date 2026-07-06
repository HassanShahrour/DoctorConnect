namespace DoctorConnect.ViewModels
{
    public class RolePermissionsPageViewModel
    {
        public List<RolePermissionMatrixRoleViewModel> Roles { get; set; } = new();
        public List<RolePermissionMatrixModuleViewModel> Modules { get; set; } = new();
    }

    public class RolePermissionMatrixRoleViewModel
    {
        public string RoleId { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
    }

    public class RolePermissionMatrixModuleViewModel
    {
        public string Module { get; set; } = string.Empty;
        public List<RolePermissionMatrixItemViewModel> Permissions { get; set; } = new();
    }

    public class RolePermissionMatrixItemViewModel
    {
        public string PermissionId { get; set; } = string.Empty;
        public string PermissionName { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public Dictionary<string, bool> RoleAssignments { get; set; } = new();
    }

    public class RolePermissionsUpdateRequest
    {
        public List<RolePermissionSelectionViewModel> Selections { get; set; } = new();
    }

    public class RolePermissionSelectionViewModel
    {
        public string RoleId { get; set; } = string.Empty;
        public string PermissionId { get; set; } = string.Empty;
        public bool IsGranted { get; set; }
    }
}
