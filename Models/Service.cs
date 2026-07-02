namespace DoctorConnect.Models
{
    public class Service : BaseEntity
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public decimal? Fees { get; set; }
        public bool IsActive { get; set; } = true;

        public string? DoctorId { get; set; }
        public Doctor? Doctor { get; set; }

        public string? ClinicId { get; set; }
        public Clinic? Clinic { get; set; }
    }
}
