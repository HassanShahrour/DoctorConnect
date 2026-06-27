using Microsoft.AspNetCore.Mvc.Rendering;

namespace DoctorConnect.ViewModels
{
    public class DoctorAvailabilityViewModel
    {
        public string DoctorId { get; set; }
        public List<ClinicHeaderViewModel> Clinics { get; set; } = new();
        public List<DoctorAvailabilityDayRowViewModel> Days { get; set; } = new();
    }

    public class ClinicHeaderViewModel
    {
        public string ClinicId { get; set; }
        public string? ClinicName { get; set; }
        public List<SelectListItem> ClinicOptions { get; set; } = new();
    }

    public class DoctorAvailabilityDayRowViewModel
    {
        public DayOfWeek DayOfWeek { get; set; }
        public List<DoctorAvailabilityCellViewModel> PerClinicAvailabilities { get; set; } = new();
    }

    public class DoctorAvailabilityCellViewModel
    {
        public string ClinicId { get; set; }
        public bool IsAvailable { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public int DurationInMinutes { get; set; }
        public List<BreakTimeViewModel> BreakTimes { get; set; } = new();
    }

    public class BreakTimeViewModel
    {
        public TimeSpan? Start { get; set; }
        public TimeSpan? End { get; set; }
    }
}
