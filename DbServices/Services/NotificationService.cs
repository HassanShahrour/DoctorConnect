using DoctorConnect.DbServices.IServices;
using DoctorConnect.Models;
using DoctorConnect.Repositories;

namespace DoctorConnect.DbServices.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IGenericRepository<Notification> _repo;

        public NotificationService(IGenericRepository<Notification> repo)
        {
            _repo = repo;
        }

        public async Task CreateAsync(Notification notification)
        {
            await _repo.AddAsync(notification);
            await _repo.SaveChangesAsync();
        }

        public async Task<IEnumerable<Notification>> GetForUserAsync(string userId)
        {
            var all = await _repo.GetAllAsync();
            return all.Where(n => n.UserId == userId);
        }

        public async Task MarkAsReadAsync(string id)
        {
            var n = await _repo.GetByIdAsync(id);
            if (n != null)
            {
                n.IsRead = true;
                await _repo.UpdateAsync(n);
                await _repo.SaveChangesAsync();
            }
        }
    }
}
