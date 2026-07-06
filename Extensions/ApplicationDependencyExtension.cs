using DoctorConnect.Authorization;
using DoctorConnect.DbServices.IServices;
using DoctorConnect.DbServices.Services;
using DoctorConnect.Repositories;
using DoctorConnect.Utilities.IServices;
using DoctorConnect.Utilities.Services;
using Microsoft.AspNetCore.Authorization;

namespace DoctorConnect.Extensions
{
    public static class ApplicationDependencyExtension
    {
        public static IServiceCollection AddInterfaceScopeServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IDoctorService, DoctorService>();
            services.AddScoped<IDoctorTaskService, DoctorTaskService>();
            services.AddScoped<IPatientService, PatientService>();
            services.AddScoped<IRelativeService, RelativeService>();
            services.AddScoped<IAppointmentService, AppointmentService>();
            services.AddScoped<IMedicalRecordService, MedicalRecordService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IDashboardService, DashboardService>();
            services.AddScoped<IAvailabilityService, AvailabilityService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<ISpecialityService, SpecialityService>();
            services.AddScoped<IClinicService, ClinicService>();
            services.AddScoped<IServiceService, ServiceService>();
            services.AddScoped<ISettingsService, SettingsService>();
            services.AddScoped<IPermissionService, PermissionService>();
            services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
            services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
            return services;
        }
    }
}
