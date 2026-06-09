using System.ComponentModel.DataAnnotations;

namespace DoctorConnect.Models
{
    public abstract class BaseEntity
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
