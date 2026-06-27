namespace DoctorConnect.ViewModels
{
    public class DoctorAvailabilityViewModel
    {
        public string DoctorId { get; set; }
        public List<ClinicAvailabilityViewModel> Clinics { get; set; } = new();
    }

    public class ClinicAvailabilityViewModel
    {
        public string ClinicId { get; set; }
        public string ClinicName { get; set; }
        public List<DoctorAvailabilityDayViewModel> Days { get; set; } = new();
    }
}
