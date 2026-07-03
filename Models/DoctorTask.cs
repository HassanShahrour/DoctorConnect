using System.ComponentModel.DataAnnotations;

namespace DoctorConnect.Models
{
    public class DoctorTask : BaseEntity
    {
        [Required]
        public string Name { get; set; }
        public string? Description { get; set; }

        [Range(0, 100)]
        public int Progress { get; set; }

        public TaskStatusEnum Status { get; set; } = TaskStatusEnum.Pending;

        [Required]
        public DateTime TaskDate { get; set; } = DateTime.Today;

        [Required]
        public string DoctorId { get; set; }
        public Doctor Doctor { get; set; }

        public List<TaskBullet> Bullets { get; set; } = new();
    }
}
