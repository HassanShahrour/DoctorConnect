using Microsoft.AspNetCore.Mvc.Rendering;

namespace DoctorConnect.ViewModels
{
    public class CreateDoctorViewModel
    {
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string? Qualifications { get; set; }
        public int YearsOfExperience { get; set; }
        public string? Biography { get; set; }
        public decimal? ConsultationFee { get; set; }
        public string? ProfilePhoto { get; set; }
        public bool IsActive { get; set; } = true;
        public string? ClinicId { get; set; }
        public string SpecialtyId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Address { get; set; }
        public IEnumerable<SelectListItem>? Specialities { get; set; }
        public IEnumerable<SelectListItem>? Clinics { get; set; }
    }
}
