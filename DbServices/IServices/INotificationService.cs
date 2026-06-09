using DoctorConnect.Models;

namespace DoctorConnect.DbServices.IServices
{
    public interface INotificationService
    {
        Task<IEnumerable<Notification>> GetForUserAsync(string userId);
        Task CreateAsync(Notification notification);
        Task MarkAsReadAsync(string id);
    }
}
