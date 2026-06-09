namespace DoctorConnect.ViewModels
{
    public class DoctorAvailabilityViewModel
    {
        public string DoctorId { get; set; }
        public List<DoctorAvailabilityDayViewModel> Days { get; set; } = new List<DoctorAvailabilityDayViewModel>();
    }
}
