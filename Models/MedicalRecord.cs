namespace DoctorConnect.Models
{
    public class MedicalRecord : BaseEntity
    {
        public string PatientId { get; set; }
        public string DoctorId { get; set; }
        public Doctor Doctor { get; set; }
        public string Diagnosis { get; set; }
        public string Prescription { get; set; }
        public string Notes { get; set; }
        public DateTime VisitDate { get; set; }
        public Patient Patient { get; set; }
        public List<MedicalAttachment> Attachments { get; set; }
    }
}
