using System.ComponentModel.DataAnnotations;

namespace DoctorConnect.Models
{
    public class Appointment : BaseEntity
    {
        [Required]
        public string? DoctorId { get; set; }
        public string? PatientId { get; set; }
        public string? ClinicId { get; set; }
        [Required]
        public DateTime AppointmentDate { get; set; }
        [Required]
        public TimeSpan AppointmentTime { get; set; }
        public string? Notes { get; set; }
        [Range(0, 100000)]
        public decimal? Fees { get; set; }
        public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;

        public bool IsGuestPatient { get; set; }
        public string? GuestPatientFullName { get; set; }
        public string? GuestPatientPhoneNumber { get; set; }
        public Gender? GuestPatientGender { get; set; }
        public DateTime? GuestPatientDateOfBirth { get; set; }

        public Doctor? Doctor { get; set; }
        public Patient? Patient { get; set; }
    }
}