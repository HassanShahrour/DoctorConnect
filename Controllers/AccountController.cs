using DoctorConnect.DbServices.IServices;
using DoctorConnect.DTOs;
using DoctorConnect.Models;
using DoctorConnect.Utilities.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DoctorConnect.Controllers
{
    public class AccountController : Controller
    {
        private readonly IEmailService _emailService;
        private readonly IAccountService _accountService;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountController(
            IEmailService emailService,
            IAccountService accountService,
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager)
        {
            _emailService = emailService;
            _accountService = accountService;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginDTO model)
        {
            if (!ModelState.IsValid) return View(model);
            var user = await _userManager.FindByEmailAsync(model.Email);
            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
            if (result.Succeeded)
            {
                var roles = await _userManager.GetRolesAsync(user);
                if (roles.Contains("Admin"))
                    return RedirectToAction("Index", "Home");
                return RedirectToAction("Index", "Home");
            }
            ModelState.AddModelError(string.Empty, "Invalid credentials!");
            return View(model);
        }

        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(RegisterDTO model)
        {
            if (!ModelState.IsValid) return View(model);
            var result = await _accountService.RegisterPatient(model);
            if (result.Succeeded)
            {
                await Login(new LoginDTO
                {
                    Email = model.Email,
                    Password = model.Password,
                    RememberMe = false
                });
            }
            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);
            return View(model);
        }

        public IActionResult VerifyEmail() => View();

        [HttpPost]
        public async Task<IActionResult> VerifyEmail(VerifyEmailDTO model)
        {
            if (!ModelState.IsValid) return View(model);
            var user = await _userManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Something is wrong!");
                return View(model);
            }
            var otp = new Random().Next(100000, 999999).ToString();
            HttpContext.Session.SetString("OTP_Email", model.Email);
            HttpContext.Session.SetString("OTP_Code", otp);
            HttpContext.Session.SetString("OTP_Expiry", DateTime.UtcNow.AddMinutes(5).ToString("O"));
            await _emailService.SendEmailAsync(model.Email, "Your OTP Code", $"Your OTP is: {otp}");
            return RedirectToAction("VerifyOtp");
        }

        [HttpGet]
        public IActionResult VerifyOtp()
        {
            return View();
        }

        [HttpPost]
        public IActionResult VerifyOtp(VerifyOtpDTO model)
        {
            if (!ModelState.IsValid) return View(model);
            var storedOtp = HttpContext.Session.GetString("OTP_Code");
            var storedEmail = HttpContext.Session.GetString("OTP_Email");
            var expiryString = HttpContext.Session.GetString("OTP_Expiry");
            if (storedOtp == null || storedEmail == null || expiryString == null)
            {
                ModelState.AddModelError("", "OTP session expired. Please try again.");
                return View(model);
            }
            var expiry = DateTime.Parse(expiryString);
            if (DateTime.UtcNow > expiry)
            {
                ModelState.AddModelError("", "OTP expired. Please request again.");
                return View(model);
            }
            if (model.OtpCode != storedOtp)
            {
                ModelState.AddModelError("", "Invalid OTP.");
                return View(model);
            }
            HttpContext.Session.Remove("OTP_Code");
            HttpContext.Session.Remove("OTP_Expiry");
            return RedirectToAction("ChangePassword", new { username = storedEmail });
        }

        public IActionResult ChangePassword(string username)
        {
            if (string.IsNullOrEmpty(username))
                return RedirectToAction("Index", "Home");
            return View(new ChangePasswordDTO { Email = username });
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordDTO model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "Something went wrong. Try again.");
                return View(model);
            }
            var user = await _userManager.FindByNameAsync(model.Email ?? "");
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Email not found!");
                return View(model);
            }
            var removeResult = await _userManager.RemovePasswordAsync(user);
            if (!removeResult.Succeeded)
            {
                foreach (var error in removeResult.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
                return View(model);
            }
            var addResult = await _userManager.AddPasswordAsync(user, model.NewPassword);
            if (addResult.Succeeded)
                return RedirectToAction("Login");
            foreach (var error in addResult.Errors)
                ModelState.AddModelError(string.Empty, error.Description);
            return View(model);
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }

    }
}
