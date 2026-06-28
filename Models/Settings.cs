namespace DoctorConnect.Models
{
    public class Settings : BaseEntity
    {
        public int NumberOfDaysToDisplay { get; set; } = 14;
    }
}