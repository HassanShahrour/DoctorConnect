namespace DoctorConnect.Models
{
    public class Doctor : BaseEntity
    {
        public string UserId { get; set; }
        public string? Qualifications { get; set; }
        public int YearsOfExperience { get; set; }
        public string? Biography { get; set; }
        public decimal? ConsultationFee { get; set; }
        public string? ProfilePhoto { get; set; }
        public bool IsActive { get; set; } = true;
        public string? ClinicId { get; set; }
        public string SpecialtyId { get; set; }
        public ApplicationUser User { get; set; }
        public List<DoctorAvailability> Availabilities { get; set; }
        public List<Appointment> Appointments { get; set; }
        public List<MedicalRecord> MedicalRecords { get; set; }
        public Clinic Clinic { get; set; }
        public Specialty Specialty { get; set; }
    }
}
