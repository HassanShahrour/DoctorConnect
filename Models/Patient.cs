namespace DoctorConnect.Models
{
    public class Patient : BaseEntity
    {
        public string UserId { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public ApplicationUser User { get; set; }
        public List<Appointment> Appointments { get; set; }
        public List<MedicalRecord> MedicalRecords { get; set; }
    }
}
