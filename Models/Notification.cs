namespace DoctorConnect.Models
{
    public class Notification : BaseEntity
    {
        public string UserId { get; set; }
        public string Message { get; set; }
        public bool IsRead { get; set; }
        public ApplicationUser User { get; set; }
    }
}
