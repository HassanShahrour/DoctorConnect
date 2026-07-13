namespace DoctorConnect.Models
{
    public enum AppointmentStatus
    {
        Pending = 1,
        Confirmed = 2,
        Completed = 3,
        Cancelled = 4,
        NoShow = 5,
        Rejected = 6
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
    public enum Relationship
    {
        Father = 1,
        Mother = 2,
        Husband = 3,
        Wife = 4,
        Son = 5,
        Daughter = 6,
        Brother = 7,
        Sister = 8,
        Other = 9
    }
}
