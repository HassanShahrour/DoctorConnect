using System.ComponentModel.DataAnnotations;

namespace DoctorConnect.DTOs
{
    public class VerifyOtpDTO
    {
        [Required]
        public string OtpCode { get; set; }
    }
}
