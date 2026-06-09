using DoctorConnect.Models;
using FluentValidation;

namespace DoctorConnect.Validators
{
    public class ApplicationUserValidator : AbstractValidator<ApplicationUser>
    {
        public ApplicationUserValidator()
        {
            RuleFor(x => x.FirstName).NotEmpty().WithMessage("First name is required");
            RuleFor(x => x.LastName).NotEmpty().WithMessage("Last name is required");
            RuleFor(x => x.Email).NotEmpty().EmailAddress().WithMessage("Valid email is required");
            RuleFor(x => x.PhoneNumber).NotEmpty().WithMessage("Phone number is required");
            RuleFor(x => x.DateOfBirth).LessThanOrEqualTo(DateTime.UtcNow).When(x => x.DateOfBirth.HasValue).WithMessage("Date of birth cannot be in the future");
        }
    }
}
