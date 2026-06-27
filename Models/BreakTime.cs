using System;

namespace DoctorConnect.Models
{
    public class BreakTime : BaseEntity
    {
        public string DoctorAvailabilityId { get; set; }
        public TimeSpan Start { get; set; }
        public TimeSpan End { get; set; }
        public DoctorAvailability DoctorAvailability { get; set; }
    }
}
