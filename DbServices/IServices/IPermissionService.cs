using DoctorConnect.ViewModels;

namespace DoctorConnect.DbServices.IServices
{
    public interface IPermissionService
    {
        Task SeedAsync();
        Task<IReadOnlyCollection<string>> GetUserPermissionsAsync(string userId);
        Task<RolePermissionsPageViewModel> GetRolePermissionsAsync();
        Task UpdateRolePermissionsAsync(RolePermissionsUpdateRequest request, string? changedByUserId);
    }
}
