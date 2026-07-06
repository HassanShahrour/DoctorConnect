using System.ComponentModel.DataAnnotations;

namespace DoctorConnect.Models
{
    public class Relative : BaseEntity
    {
        [Required]
        public string PatientId { get; set; } = string.Empty;

        [Required]
        public string FullName { get; set; } = string.Empty;

        public string? PhoneNumber { get; set; }

        public Relationship Relationship { get; set; }

        public Gender Gender { get; set; }

        public DateTime DateOfBirth { get; set; }

        public Patient? Patient { get; set; }
    }
}
