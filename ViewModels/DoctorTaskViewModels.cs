using DoctorConnect.Models;
using System.ComponentModel.DataAnnotations;

namespace DoctorConnect.ViewModels
{
    public class DoctorTaskManagementViewModel
    {
        public string? Id { get; set; }
        public string? DoctorId { get; set; }
        public string? DoctorName { get; set; }
        public IEnumerable<DoctorTask> Tasks { get; set; } = Enumerable.Empty<DoctorTask>();

        [Required]
        public string Name { get; set; }

        public string? Description { get; set; }

        [Range(0, 100)]
        public int Progress { get; set; }

        [Required]
        public DateTime TaskDate { get; set; } = DateTime.Today;

        public TaskStatusEnum Status { get; set; } = TaskStatusEnum.Pending;

        public List<TaskBulletInputViewModel> Bullets { get; set; } = new();
    }

    public class TaskBulletInputViewModel
    {
        public string? Id { get; set; }
        public string? Description { get; set; }
        public bool IsChecked { get; set; }
    }
}
