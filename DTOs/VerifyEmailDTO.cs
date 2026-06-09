using System.ComponentModel.DataAnnotations;

namespace DoctorConnect.DTOs
{
    public class VerifyEmailDTO
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress]
        public string Email { get; set; }
    }
}
