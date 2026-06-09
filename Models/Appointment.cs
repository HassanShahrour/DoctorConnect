using static DoctorConnect.Models.Enums;

namespace DoctorConnect.Models
{
    public class Appointment : BaseEntity
    {
        public string DoctorId { get; set; }
        public string PatientId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public TimeSpan AppointmentTime { get; set; }
        public string Notes { get; set; }
        public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;
        public Doctor Doctor { get; set; }
        public Patient Patient { get; set; }
    }
}