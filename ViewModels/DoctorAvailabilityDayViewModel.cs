namespace DoctorConnect.ViewModels
{
    public class DoctorAvailabilityDayViewModel
    {
        public DayOfWeek DayOfWeek { get; set; }
        public bool IsAvailable { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public int DurationInMinutes { get; set; }
    }
}
