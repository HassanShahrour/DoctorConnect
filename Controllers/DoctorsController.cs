using DoctorConnect.Authorization;
using DoctorConnect.DbServices.IServices;
using DoctorConnect.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DoctorConnect.Controllers
{
    [Authorize]
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

        [Authorize(Policy = AppPermissions.Doctors.Read)]
        public async Task<IActionResult> Index()
        {
            try
            {
                var docs = await _doctorService.GetAllAsync();
                return View(docs);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in DoctorsController.Index: {ex}");
                TempData["Error"] = $"An error occurred loading doctors: {ex.Message}";
                return View(Enumerable.Empty<DoctorConnect.Models.Doctor>());
            }
        }

        [Authorize(Policy = AppPermissions.Doctors.Read)]
        public async Task<IActionResult> Browse()
        {
            try
            {
                var docs = await _doctorService.GetAllAsync();
                return View(docs);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in DoctorsController.Browse: {ex}");
                TempData["Error"] = $"An error occurred loading doctors: {ex.Message}";
                return View(Enumerable.Empty<DoctorConnect.Models.Doctor>());
            }
        }

        [Authorize(Policy = AppPermissions.Doctors.Create)]
        public IActionResult Create()
        {
            try
            {
                var model = new CreateDoctorViewModel
                {
                    Specialities = FetchSpecialities(),
                    Clinics = FetchClinics(),
                };
                return View(model);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in DoctorsController.Create [GET]: {ex}");
                TempData["Error"] = $"An error occurred initializing view: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = AppPermissions.Doctors.Create)]
        public async Task<IActionResult> Create(CreateDoctorViewModel model)
        {
            try
            {
                var postedClinicIds = Request.Form["ClinicIds"].ToList();
                if (postedClinicIds != null && postedClinicIds.Count > 0)
                {
                    model.ClinicIds = postedClinicIds;
                }
                else if (Request.Form.ContainsKey("ClinicIdsComma"))
                {
                    var comma = Request.Form["ClinicIdsComma"].ToString();
                    model.ClinicIds = (comma ?? string.Empty).Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToList();
                }

                if (!ModelState.IsValid)
                {
                    model.Specialities = FetchSpecialities();
                    model.Clinics = FetchClinics();
                    TempData["Error"] = "Please correct errors in form fields.";
                    return View(model);
                }
                var result = await _accountService.RegisterDoctor(model);
                if (result.Succeeded)
                {
                    TempData["Success"] = "Doctor registered successfully.";
                    return RedirectToAction(nameof(Index));
                }
                foreach (var err in result.Errors)
                    ModelState.AddModelError(string.Empty, err.Description);
                model.Specialities = FetchSpecialities();
                model.Clinics = FetchClinics();
                return View(model);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in DoctorsController.Create [POST]: {ex}");
                TempData["Error"] = $"An error occurred: {ex.Message}";
                model.Specialities = FetchSpecialities();
                model.Clinics = FetchClinics();
                return View(model);
            }
        }

        [Authorize(Policy = AppPermissions.Doctors.Update)]
        public async Task<IActionResult> Edit(string id)
        {
            try
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
                    ClinicIds = doctor.Clinics?.Select(c => c.Id).ToList() ?? new List<string>(),
                    SpecialtyId = doctor.SpecialtyId,
                    Specialities = FetchSpecialities(),
                    Clinics = FetchClinics(),
                };
                return View(model);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in DoctorsController.Edit [GET]: {ex}");
                TempData["Error"] = $"An error occurred loading details: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = AppPermissions.Doctors.Update)]
        public async Task<IActionResult> Edit(EditDoctorViewModel model)
        {
            try
            {
                var postedClinicIds = Request.Form["ClinicIds"].ToList();
                if (postedClinicIds != null && postedClinicIds.Count > 0)
                {
                    model.ClinicIds = postedClinicIds;
                }
                else if (Request.Form.ContainsKey("ClinicIdsComma"))
                {
                    var comma = Request.Form["ClinicIdsComma"].ToString();
                    model.ClinicIds = (comma ?? string.Empty).Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToList();
                }

                if (!ModelState.IsValid)
                {
                    model.Specialities = FetchSpecialities();
                    model.Clinics = FetchClinics();
                    TempData["Error"] = "Form validation failed. Please check your details.";
                    return View(model);
                }
                await _doctorService.UpdateAsync(model);
                TempData["Success"] = "Doctor details updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in DoctorsController.Edit [POST]: {ex}");
                TempData["Error"] = $"An error occurred: {ex.Message}";
                model.Specialities = FetchSpecialities();
                model.Clinics = FetchClinics();
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = AppPermissions.Doctors.Delete)]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                await _doctorService.DeleteAsync(id);
                TempData["Success"] = "Doctor deleted successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in DoctorsController.Delete: {ex}");
                TempData["Error"] = $"An error occurred: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        [Authorize(Policy = AppPermissions.Appointments.Read)]
        public async Task<IActionResult> Appointments(string doctorId)
        {
            try
            {
                var doctor = await _doctorService.GetByIdAsync(doctorId);
                if (doctor == null) return NotFound();

                var appointments = (doctor.Appointments ?? new List<DoctorConnect.Models.Appointment>())
                    .OrderBy(a => a.AppointmentDate)
                    .ThenBy(a => a.AppointmentTime)
                    .ToList();

                var model = new AppointmentViewModel
                {
                    DoctorId = doctorId,
                    DoctorName = string.Join(" ", new[] { doctor.User?.FirstName, doctor.User?.LastName }.Where(x => !string.IsNullOrWhiteSpace(x))),
                    Appointments = appointments
                };
                return View("~/Views/Appointments/Index.cshtml", model);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in DoctorsController.Appointments: {ex}");
                TempData["Error"] = $"An error occurred: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
