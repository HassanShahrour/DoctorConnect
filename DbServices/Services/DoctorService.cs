using DoctorConnect.DbServices.IServices;
using DoctorConnect.Models;
using DoctorConnect.Repositories;
using DoctorConnect.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace DoctorConnect.DbServices.Services
{
    public class DoctorService : IDoctorService
    {
        private readonly IGenericRepository<Doctor> _repo;
        private readonly IGenericRepository<ApplicationUser> _accountRepo;
        private readonly IGenericRepository<Clinic> _clinicRepo;
        public DoctorService(IGenericRepository<Doctor> repo, IGenericRepository<ApplicationUser> accountRepo, IGenericRepository<Clinic> clinicRepo)
        {
            _repo = repo;
            _accountRepo = accountRepo;
            _clinicRepo = clinicRepo;
        }

        public async Task CreateAsync(Doctor doctor)
        {
            await _repo.AddAsync(doctor);
            await _repo.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            await _repo.ExecuteInTransactionAsync(async () =>
            {
                var doctor = await _repo.GetByIdAsync(id);
                var user = await _accountRepo.GetByIdAsync(doctor.UserId);
                if (doctor != null)
                    await _repo.RemoveAsync(doctor);
                if (user != null)
                    await _accountRepo.RemoveAsync(user);
            });
        }

        public async Task<IEnumerable<Doctor>> GetAllAsync()
        {
            return await _repo.GetAllAsync(q => q
                        .Include(d => d.User)
                        .Include(d => d.Specialty)
                        .Include(d => d.Clinics));
        }

        public async Task<Doctor> GetByIdAsync(string id)
        {
            return await _repo.GetByIdAsync(id, q => q
                        .Include(d => d.User)
                        .Include(d => d.Availabilities)
                        .Include(d => d.Appointments)
                        .ThenInclude(a => a.Patient)
                        .ThenInclude(p => p.User)
                        .Include(d => d.Specialty)
                        .Include(d => d.Clinics));
        }

        public async Task UpdateAsync(EditDoctorViewModel model)
        {
            await _repo.ExecuteInTransactionAsync(async () =>
            {
                var doctor = await _repo.GetByIdAsync(model.Id, q => q.Include(d => d.Clinics));
                var user = await _accountRepo.GetByIdAsync(model.UserId);

                if (doctor != null && user != null)
                {
                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    user.Email = model.Email;
                    user.NormalizedEmail = model.Email.ToUpper();
                    user.UserName = model.Email;
                    user.NormalizedUserName = model.Email.ToUpper();
                    user.PhoneNumber = model.PhoneNumber;
                    user.Gender = model.Gender;
                    user.DateOfBirth = model.DateOfBirth;
                    user.Address = model.Address;

                    doctor.Qualifications = model.Qualifications;
                    doctor.YearsOfExperience = model.YearsOfExperience;
                    doctor.Biography = model.Biography;
                    doctor.ConsultationFee = model.ConsultationFee;
                    doctor.ProfilePhoto = model.ProfilePhoto;
                    doctor.IsActive = model.IsActive;
                    doctor.SpecialtyId = model.SpecialtyId;

                    // Update clinics
                    doctor.Clinics.Clear();
                    if (model.ClinicIds != null)
                    {
                        foreach (var clinicId in model.ClinicIds)
                        {
                            var clinic = await _clinicRepo.GetByIdAsync(clinicId);
                            if (clinic != null)
                                doctor.Clinics.Add(clinic);
                        }
                    }

                    await _repo.UpdateAsync(doctor);
                    await _accountRepo.UpdateAsync(user);
                }
            });
        }

    }
}
