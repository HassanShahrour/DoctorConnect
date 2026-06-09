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
        public DoctorService(IGenericRepository<Doctor> repo, IGenericRepository<ApplicationUser> accountRepo)
        {
            _repo = repo;
            _accountRepo = accountRepo;
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
                        .Include(d => d.Clinic));
        }

        public async Task<Doctor> GetByIdAsync(string id)
        {
            return await _repo.GetByIdAsync(id, q => q
                        .Include(d => d.User)
                        .Include(d => d.Specialty)
                        .Include(d => d.Clinic));
        }

        public async Task UpdateAsync(EditDoctorViewModel model)
        {
            await _repo.ExecuteInTransactionAsync(async () =>
            {
                var doctor = await _repo.GetByIdAsync(model.Id);
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
                    doctor.ClinicId = model.ClinicId;

                    await _repo.UpdateAsync(doctor);
                    await _accountRepo.UpdateAsync(user);
                }
            });
        }

    }
}
