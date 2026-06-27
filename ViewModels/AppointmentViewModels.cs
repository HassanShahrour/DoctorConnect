using DoctorConnect.Models;

namespace DoctorConnect.ViewModels
{
    public class AppointmentViewModel
    {
        public IEnumerable<Appointment> Appointments { get; set; }
        public string Search { get; set; }
        public string StatusFilter { get; set; }
        public string PatientFilter { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalCount { get; set; }
    }
}
