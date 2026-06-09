namespace DoctorConnect.Models
{
    public class DoctorAvailability : BaseEntity
    {
        public string DoctorId { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int DurationInMinutes { get; set; }
        public bool IsAvailable { get; set; } = true;
        public Doctor Doctor { get; set; }
    }
}