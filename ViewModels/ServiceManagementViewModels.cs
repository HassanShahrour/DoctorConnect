using DoctorConnect.Models;
using System.ComponentModel.DataAnnotations;

namespace DoctorConnect.ViewModels
{
    public class ServiceManagementViewModel
    {
        public string? OwnerId { get; set; }
        public string? OwnerName { get; set; }
        public string? OwnerType { get; set; }
        public string? Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string? Description { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Fees must be a positive value.")]
        public decimal? Fees { get; set; }

        public bool IsActive { get; set; } = true;

        public string? DoctorId { get; set; }
        public string? ClinicId { get; set; }
        public IEnumerable<Service> Services { get; set; } = Enumerable.Empty<Service>();

    }
}
