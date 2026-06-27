using DoctorConnect.DbServices.IServices;
using DoctorConnect.DTOs;
using DoctorConnect.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DoctorConnect.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class AccountApiController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountApiController(
            IAccountService accountService,
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager)
        {
            _accountService = accountService;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        // POST: api/AccountApi/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO model)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Validation failed",
                    Data = null
                });
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return Unauthorized(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Invalid credentials"
                });
            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, lockoutOnFailure: false);
            if (!result.Succeeded)
                return Unauthorized(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Invalid credentials"
                });
            var roles = await _userManager.GetRolesAsync(user);
            var id = "";
            if (roles.Contains("Patient"))
            {
                var patient = await _accountService.GetPatientByUserId(user.Id);
                if (patient != null)
                {
                    id = patient.Id;
                }
            }
            if (string.IsNullOrEmpty(id))
            {
                return Unauthorized(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Invalid credentials"
                });
            }
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Login successful",
                Data = new UserLoginData
                {
                    Id = id,
                    Email = user.Email,
                    Roles = roles.ToList()
                }
            });
        }

        // POST: api/AccountApi/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO model)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Validation failed",
                    Data = null
                });
            var result = await _accountService.RegisterPatient(model);
            if (result.Succeeded)
            {
                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Registration successful",
                    Data = null
                });
            }
            var errors = result.Errors.Select(e => e.Description).ToArray();
            return BadRequest(new ApiResponse<object>
            {
                Success = false,
                Message = string.Join("; ", errors),
                Data = null
            });
        }
    }
}
