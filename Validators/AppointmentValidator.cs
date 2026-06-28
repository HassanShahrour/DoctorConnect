using DoctorConnect.Models;
using FluentValidation;

namespace DoctorConnect.Validators
{
    public class AppointmentValidator : AbstractValidator<Appointment>
    {
        public AppointmentValidator()
        {
            RuleFor(x => x.DoctorId).NotEmpty().WithMessage("Doctor is required");

            When(x => !x.IsGuestPatient, () =>
            {
                RuleFor(x => x.PatientId).NotEmpty().WithMessage("Patient is required when not a guest");
            });

            When(x => x.IsGuestPatient, () =>
            {
                RuleFor(x => x.GuestPatientFullName).NotEmpty().WithMessage("Guest full name is required");
                RuleFor(x => x.GuestPatientPhoneNumber).NotEmpty().WithMessage("Guest phone number is required");
                RuleFor(x => x.GuestPatientGender).NotNull().WithMessage("Guest gender is required");
                RuleFor(x => x.GuestPatientDateOfBirth).NotNull().WithMessage("Guest date of birth is required");
            });

            RuleFor(x => x.AppointmentDate).GreaterThanOrEqualTo(DateTime.UtcNow.Date).WithMessage("Appointment date cannot be in the past");
            RuleFor(x => x.AppointmentTime).NotNull().WithMessage("Appointment time is required");
        }
    }
}
