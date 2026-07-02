namespace DoctorConnect.Models
{
    public class Clinic : BaseEntity
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; } = true;
        public List<Doctor> Doctors { get; set; }
        public List<Service> Services { get; set; }
    }
}