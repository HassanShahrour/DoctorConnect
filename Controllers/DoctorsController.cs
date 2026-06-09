using DoctorConnect.DbServices.IServices;
using DoctorConnect.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DoctorConnect.Controllers
{
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class DoctorsController : Controller
    {
        private readonly IDoctorService _doctorService;
        private readonly IAccountService _accountService;
        private readonly ISpecialityService _specialityService;
        private readonly IClinicService _clinicService;
        public DoctorsController(IDoctorService doctorService, IAccountService accountService, ISpecialityService specialityService, IClinicService clinicService)
        {
            _doctorService = doctorService;
            _accountService = accountService;
            _specialityService = specialityService;
            _clinicService = clinicService;
        }

        private List<SelectListItem> FetchSpecialities()
        {
            return _specialityService.GetAllAsync().Result.Select(s => new SelectListItem
            {
                Value = s.Id,
                Text = s.Name
            }).ToList();
        }
        private List<SelectListItem> FetchClinics()
        {
            return _clinicService.GetAllAsync().Result.Select(s => new SelectListItem
            {
                Value = s.Id,
                Text = s.Name
            }).ToList();
        }

        public async Task<IActionResult> Index()
        {
            var docs = await _doctorService.GetAllAsync();
            return View(docs);
        }

        public async Task<IActionResult> Browse()
        {
            var docs = await _doctorService.GetAllAsync();
            return View(docs);
        }

        public IActionResult Create()
        {
            var model = new CreateDoctorViewModel
            {
                Specialities = FetchSpecialities(),
                Clinics = FetchClinics(),
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateDoctorViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Specialities = FetchSpecialities();
                model.Clinics = FetchClinics();
                return View(model);
            }
            var result = await _accountService.RegisterDoctor(model);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Index));
            }
            foreach (var err in result.Errors)
                ModelState.AddModelError(string.Empty, err.Description);
            model.Specialities = FetchSpecialities();
            model.Clinics = FetchClinics();
            return View(model);
        }

        public async Task<IActionResult> Edit(string id)
        {
            var doctor = await _doctorService.GetByIdAsync(id);
            if (doctor == null) return NotFound();
            var model = new EditDoctorViewModel
            {
                Id = doctor.Id,
                UserId = doctor.UserId,
                Email = doctor.User?.Email,
                PhoneNumber = doctor.User?.PhoneNumber,
                FirstName = doctor.User?.FirstName,
                LastName = doctor.User?.LastName,
                Gender = doctor.User?.Gender,
                DateOfBirth = doctor.User?.DateOfBirth,
                Address = doctor.User?.Address,
                Qualifications = doctor.Qualifications,
                YearsOfExperience = doctor.YearsOfExperience,
                Biography = doctor.Biography,
                ConsultationFee = doctor.ConsultationFee,
                ProfilePhoto = doctor.ProfilePhoto,
                IsActive = doctor.IsActive,
                ClinicId = doctor.ClinicId,
                SpecialtyId = doctor.SpecialtyId,
                Specialities = FetchSpecialities(),
                Clinics = FetchClinics(),
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditDoctorViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            await _doctorService.UpdateAsync(model);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            await _doctorService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
