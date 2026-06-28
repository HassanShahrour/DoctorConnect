using DoctorConnect.Models;

namespace DoctorConnect.ViewModels
{
    public class AppointmentViewModel
    {
        public string DoctorId { get; set; }
        public string? DoctorName { get; set; }
        public IEnumerable<Appointment> Appointments { get; set; } = Enumerable.Empty<Appointment>();
        public IEnumerable<AppointmentCalendarDayViewModel> CalendarDays { get; set; } = Enumerable.Empty<AppointmentCalendarDayViewModel>();
        public string Search { get; set; }
        public string StatusFilter { get; set; }
        public string PatientFilter { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalCount { get; set; }
    }

    public class AppointmentCalendarDayViewModel
    {
        public DateTime Date { get; set; }
        public int AppointmentCount { get; set; }
        public IEnumerable<AppointmentCalendarItemViewModel> Appointments { get; set; } = Enumerable.Empty<AppointmentCalendarItemViewModel>();
    }

    public class AppointmentCalendarItemViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string PatientName { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public string Status { get; set; } = string.Empty;
        public string TimeDisplay { get; set; } = string.Empty;
        public decimal? Fees { get; set; }
        public bool IsGuestPatient { get; set; }
    }
}
