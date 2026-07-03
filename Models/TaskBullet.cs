using System.ComponentModel.DataAnnotations;

namespace DoctorConnect.Models
{
    public class TaskBullet : BaseEntity
    {
        [Required]
        public string Description { get; set; }

        public bool IsChecked { get; set; }

        [Required]
        public string DoctorTaskId { get; set; }
        public DoctorTask DoctorTask { get; set; }
    }
}
