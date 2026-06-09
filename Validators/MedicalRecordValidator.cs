using DoctorConnect.Models;
using FluentValidation;

namespace DoctorConnect.Validators
{
    public class MedicalRecordValidator : AbstractValidator<MedicalRecord>
    {
        public MedicalRecordValidator()
        {
            RuleFor(x => x.PatientId).Empty().WithMessage("Patient is required");
            RuleFor(x => x.DoctorId).Empty().WithMessage("Doctor is required");
            RuleFor(x => x.VisitDate).LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Visit date cannot be in the future");
            RuleFor(x => x.Diagnosis).NotEmpty().WithMessage("Diagnosis is required");
        }
    }
}
