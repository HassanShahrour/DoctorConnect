using DoctorConnect.Models;
using FluentValidation;

namespace DoctorConnect.Validators
{
    public class AppointmentValidator : AbstractValidator<Appointment>
    {
        public AppointmentValidator()
        {
            RuleFor(x => x.DoctorId).Empty().WithMessage("Doctor is required");
            RuleFor(x => x.PatientId).Empty().WithMessage("Patient is required");
            RuleFor(x => x.AppointmentDate).GreaterThanOrEqualTo(DateTime.UtcNow.Date).WithMessage("Appointment date cannot be in the past");
            RuleFor(x => x.AppointmentTime).NotNull().WithMessage("Appointment time is required");
        }
    }
}
