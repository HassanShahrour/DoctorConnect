using DoctorConnect.DbServices.IServices;
using DoctorConnect.DTOs;
using DoctorConnect.Models;
using DoctorConnect.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace DoctorConnect.DbServices.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IDoctorService _doctorService;
        private readonly IPatientService _patientService;

        public AccountService(
            UserManager<ApplicationUser> userManager,
            IDoctorService doctorService,
            IPatientService patientService)
        {
            _userManager = userManager;
            _doctorService = doctorService;
            _patientService = patientService;
        }

        public async Task<IdentityResult> RegisterDoctor(CreateDoctorViewModel model)
        {
            var existing = await _userManager.FindByEmailAsync(model.Email);
            if (existing != null)
            {
                return IdentityResult.Failed(
                    new IdentityError
                    {
                        Code = "EmailAlreadyExists",
                        Description = "A user with this email address already exists."
                    });
            }
            var user = new ApplicationUser
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                UserName = model.Email,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                DateOfBirth = model.DateOfBirth,
                Gender = model.Gender,
                Address = model.Address,
                EmailConfirmed = true,
            };
            var defaultPassword = "Doctor@123";
            var result = await _userManager.CreateAsync(user, defaultPassword);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Doctor");
                var doctor = new Doctor
                {
                    UserId = user.Id,
                    Biography = model.Biography,
                    YearsOfExperience = model.YearsOfExperience,
                    Qualifications = model.Qualifications,
                    SpecialtyId = model.SpecialtyId,
                    IsActive = model.IsActive,
                    ProfilePhoto = model.ProfilePhoto,
                    ConsultationFee = model.ConsultationFee,
                };
                await _doctorService.CreateAsync(doctor);
            }
            return result;
        }

        public async Task<IdentityResult> RegisterPatient(RegisterDTO model)
        {
            var existing = await _userManager.FindByEmailAsync(model.Email);
            if (existing != null)
            {
                return IdentityResult.Failed(
                    new IdentityError
                    {
                        Code = "EmailAlreadyExists",
                        Description = "A user with this email address already exists."
                    });
            }
            var user = new ApplicationUser
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                UserName = model.Email,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                DateOfBirth = model.DateOfBirth,
                Gender = model.Gender,
                Address = model.Address,
                EmailConfirmed = true,
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Patient");
                var patient = new Patient
                {
                    UserId = user.Id,
                    Longitude = model.Longitude,
                    Latitude = model.Latitude,
                };
                await _patientService.CreateAsync(patient);
            }
            return result;
        }
    }
}
