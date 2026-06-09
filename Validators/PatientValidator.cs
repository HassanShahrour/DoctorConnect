using DoctorConnect.Models;
using FluentValidation;

namespace DoctorConnect.Validators
{
    public class PatientValidator : AbstractValidator<Patient>
    {
        public PatientValidator()
        {

        }
    }
}
