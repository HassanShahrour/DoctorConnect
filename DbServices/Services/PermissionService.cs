using DoctorConnect.Authorization;
using DoctorConnect.Data;
using DoctorConnect.DbServices.IServices;
using DoctorConnect.Models;
using DoctorConnect.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace DoctorConnect.DbServices.Services
{
    public class PermissionService : IPermissionService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMemoryCache _cache;

        public PermissionService(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IMemoryCache cache)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _cache = cache;
        }

        public async Task SeedAsync()
        {
            var catalog = BuildPermissionCatalog();
            foreach (var item in catalog)
            {
                if (!await _context.Permissions.AnyAsync(p => p.Name == item.Name))
                {
                    _context.Permissions.Add(item);
                }
            }

            await _context.SaveChangesAsync();

            var roles = await _roleManager.Roles.ToListAsync();
            var permissions = await _context.Permissions.ToListAsync();

            foreach (var role in roles)
            {
                var defaults = GetDefaultPermissionsForRole(role.Name ?? string.Empty, permissions);
                foreach (var permission in defaults)
                {
                    var exists = await _context.RolePermissions.AnyAsync(rp => rp.RoleId == role.Id && rp.PermissionId == permission.Id);
                    if (!exists)
                    {
                        _context.RolePermissions.Add(new RolePermission
                        {
                            RoleId = role.Id,
                            PermissionId = permission.Id,
                            Scope = PermissionScopes.Any
                        });
                    }
                }
            }

            await _context.SaveChangesAsync();
            _cache.Remove("permission-catalog-version");
        }

        public async Task<IReadOnlyCollection<string>> GetUserPermissionsAsync(string userId)
        {
            var cacheKey = $"user-permissions:{userId}";
            if (_cache.TryGetValue(cacheKey, out IReadOnlyCollection<string>? cached) && cached is not null)
            {
                return cached;
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Array.Empty<string>();
            }

            var roles = await _userManager.GetRolesAsync(user);
            var permissions = await _context.RolePermissions
                .Where(rp => roles.Contains(rp.Role.Name!))
                .Select(rp => rp.Permission.Name)
                .Distinct()
                .ToListAsync();

            _cache.Set(cacheKey, permissions, TimeSpan.FromMilliseconds(10));
            return permissions;
        }

        public async Task<RolePermissionsPageViewModel> GetRolePermissionsAsync()
        {
            var roles = await _roleManager.Roles.OrderBy(r => r.Name).ToListAsync();
            var permissions = await _context.Permissions.OrderBy(p => p.Module).ThenBy(p => p.Action).ToListAsync();
            var assignments = await _context.RolePermissions.ToListAsync();

            return new RolePermissionsPageViewModel
            {
                Roles = roles.Select(r => new RolePermissionMatrixRoleViewModel
                {
                    RoleId = r.Id,
                    RoleName = r.Name ?? string.Empty
                }).ToList(),
                Modules = permissions
                    .GroupBy(p => p.Module)
                    .Select(g => new RolePermissionMatrixModuleViewModel
                    {
                        Module = g.Key,
                        Permissions = g.Select(p => new RolePermissionMatrixItemViewModel
                        {
                            PermissionId = p.Id,
                            PermissionName = p.Name,
                            Action = p.Action,
                            RoleAssignments = roles.ToDictionary(
                                r => r.Id,
                                r => assignments.Any(a => a.RoleId == r.Id && a.PermissionId == p.Id))
                        }).ToList()
                    }).ToList()
            };
        }

        public async Task UpdateRolePermissionsAsync(RolePermissionsUpdateRequest request, string? changedByUserId)
        {
            var existing = await _context.RolePermissions.ToListAsync();
            var lookup = existing.ToDictionary(
                x => (x.RoleId, x.PermissionId),
                x => x);
            foreach (var selection in request.Selections)
            {
                lookup.TryGetValue(
                    (selection.RoleId, selection.PermissionId),
                    out var current);
                var currentlyGranted = current != null;
                var newGranted = selection.IsGranted;
                if (newGranted && current == null)
                {
                    var entity = new RolePermission
                    {
                        RoleId = selection.RoleId,
                        PermissionId = selection.PermissionId,
                        Scope = PermissionScopes.Any,
                        CreatedByUserId = changedByUserId
                    };

                    _context.RolePermissions.Add(entity);
                }
                else if (!newGranted && current != null)
                {
                    _context.RolePermissions.Remove(current);
                }
                if (currentlyGranted != newGranted)
                {
                    _context.PermissionAuditLogs.Add(new PermissionAuditLog
                    {
                        RoleId = selection.RoleId,
                        PermissionId = selection.PermissionId,
                        OldValue = currentlyGranted,
                        NewValue = newGranted,
                        ChangedByUserId = changedByUserId,
                        Scope = PermissionScopes.Any
                    });
                }
            }
            await _context.SaveChangesAsync();
            foreach (var roleId in request.Selections
                         .Select(x => x.RoleId)
                         .Distinct())
            {
                _cache.Remove($"role-permissions:{roleId}");
            }
        }

        private static List<Permission> BuildPermissionCatalog()
        {
            return AppPermissions.All.Select(name =>
            {
                var parts = name.Split('.');
                return new Permission
                {
                    Name = name,
                    Module = parts[0],
                    Action = parts.Length > 1 ? parts[1] : "Read",
                    Description = $"Allows {name}"
                };
            }).ToList();
        }

        private static IEnumerable<Permission> GetDefaultPermissionsForRole(string roleName, List<Permission> permissions)
        {
            if (roleName.Equals("SuperAdmin", StringComparison.OrdinalIgnoreCase))
            {
                return permissions;
            }

            return roleName switch
            {
                "Admin" => permissions.Where(p => p.Module is "Doctors" or "Patients" or "Appointments" or "Services" or "Clinics" or "Specialities" || p.Name == AppPermissions.Dashboard.Admin),
                "Doctor" => permissions.Where(p => p.Module is "Tasks" or "Appointments" || p.Name == AppPermissions.Doctors.Read || p.Name == AppPermissions.Doctors.Update || p.Name == AppPermissions.Dashboard.Doctor),
                "Patient" => permissions.Where(p => p.Name == AppPermissions.Dashboard.Patient || p.Name == AppPermissions.Patients.Update || p.Name == AppPermissions.Patients.Read || p.Name == AppPermissions.Appointments.Read || p.Name == AppPermissions.Appointments.Create),
                "Secretary" => permissions.Where(p => p.Module is "Appointments" or "Patients" || p.Name == AppPermissions.Doctors.Read || p.Name == AppPermissions.Services.Read),
                _ => Enumerable.Empty<Permission>()
            };
        }
    }
}
