using DoctorConnect.DTOs;
using DoctorConnect.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace DoctorConnect.DbServices.IServices
{
    public interface IAccountService
    {
        Task<IdentityResult> RegisterPatient(RegisterDTO model);
        Task<IdentityResult> RegisterDoctor(CreateDoctorViewModel model);
    }
}
