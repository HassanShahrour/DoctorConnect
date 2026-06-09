using DoctorConnect.Models;
using FluentValidation;

namespace DoctorConnect.Validators
{
    public class DoctorValidator : AbstractValidator<Doctor>
    {
        public DoctorValidator()
        {
            RuleFor(x => x.SpecialtyId).NotEmpty().WithMessage("Specialty is required");
            RuleFor(x => x.ConsultationFee).GreaterThanOrEqualTo(0).WithMessage("Consultation fee must be >= 0");
            RuleFor(x => x.YearsOfExperience).GreaterThanOrEqualTo(0).WithMessage("Years of experience must be >= 0");
        }
    }
}
