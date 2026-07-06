using DoctorConnect.DbServices.IServices;
using DoctorConnect.Models;
using DoctorConnect.Repositories;

namespace DoctorConnect.DbServices.Services
{
    public class RelativeService : IRelativeService
    {
        private readonly IGenericRepository<Relative> _repo;
        private readonly IGenericRepository<Patient> _patientRepo;

        public RelativeService(IGenericRepository<Relative> repo, IGenericRepository<Patient> patientRepo)
        {
            _repo = repo;
            _patientRepo = patientRepo;
        }

        public async Task<List<Relative>> GetByPatientIdAsync(string patientId)
        {
            var relatives = await _repo.FindAsync(r => r.PatientId == patientId);
            return relatives
                .OrderBy(r => r.FullName)
                .ThenByDescending(r => r.CreatedAt)
                .ToList();
        }

        public async Task<Relative?> GetByIdAsync(string id)
        {
            return await _repo.GetByIdAsync(id);
        }

        public async Task<Relative> CreateAsync(Relative relative)
        {
            var patient = await _patientRepo.GetByIdAsync(relative.PatientId);
            if (patient == null)
            {
                throw new InvalidOperationException("Patient not found.");
            }

            await _repo.AddAsync(relative);
            await _repo.SaveChangesAsync();
            return relative;
        }

        public async Task<Relative?> UpdateAsync(Relative relative)
        {
            var existingRelative = await _repo.GetByIdAsync(relative.Id);
            if (existingRelative == null)
            {
                return null;
            }

            var patient = await _patientRepo.GetByIdAsync(relative.PatientId);
            if (patient == null)
            {
                throw new InvalidOperationException("Patient not found.");
            }

            existingRelative.PatientId = relative.PatientId;
            existingRelative.FullName = relative.FullName;
            existingRelative.PhoneNumber = relative.PhoneNumber;
            existingRelative.Relationship = relative.Relationship;
            existingRelative.Gender = relative.Gender;
            existingRelative.DateOfBirth = relative.DateOfBirth;

            await _repo.UpdateAsync(existingRelative);
            await _repo.SaveChangesAsync();
            return existingRelative;
        }
    }
}
