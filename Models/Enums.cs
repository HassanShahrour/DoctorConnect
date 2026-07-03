namespace DoctorConnect.Models
{
    public enum AppointmentStatus
    {
        Pending = 1,
        Confirmed = 2,
        Completed = 3,
        Cancelled = 4,
        NoShow = 5
    }

    public enum Gender
    {
        Male = 1,
        Female = 2,
        Other = 3
    }

    public enum TaskStatusEnum
    {
        Pending = 1,
        Completed = 2,
        Canceled = 3,
        InProgress = 4
    }
}
