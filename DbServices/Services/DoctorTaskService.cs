using DoctorConnect.DbServices.IServices;
using DoctorConnect.Models;
using DoctorConnect.Repositories;
using DoctorConnect.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace DoctorConnect.DbServices.Services
{
    public class DoctorTaskService : IDoctorTaskService
    {
        private readonly IGenericRepository<DoctorTask> _taskRepository;
        private readonly IGenericRepository<Doctor> _doctorRepository;

        public DoctorTaskService(IGenericRepository<DoctorTask> taskRepository, IGenericRepository<Doctor> doctorRepository)
        {
            _taskRepository = taskRepository;
            _doctorRepository = doctorRepository;
        }

        public async Task<IEnumerable<DoctorTask>> GetByDoctorIdAsync(string doctorId)
        {
            return await _taskRepository.GetAllAsync(query => query
                .Include(t => t.Bullets)
                .Where(t => t.DoctorId == doctorId));
        }

        public async Task<DoctorTask?> GetByIdAsync(string id)
        {
            return await _taskRepository.GetByIdAsync(id, query => query
                .Include(t => t.Bullets)
                .Include(t => t.Doctor)
                .ThenInclude(d => d.User));
        }

        public async Task CreateAsync(DoctorTaskManagementViewModel model)
        {
            var task = new DoctorTask
            {
                DoctorId = model.DoctorId,
                Name = model.Name,
                Description = model.Description,
                Progress = NormalizeProgress(model.Progress),
                TaskDate = model.TaskDate,
                Status = ResolveStatus(model.Progress),
                Bullets = BuildBullets(model.Bullets)
            };

            await _taskRepository.AddAsync(task);
            await _taskRepository.SaveChangesAsync();
        }

        public async Task UpdateAsync(DoctorTaskManagementViewModel model)
        {
            await _taskRepository.ExecuteInTransactionAsync(async () =>
            {
                var task = await _taskRepository.GetByIdAsync(model.Id!, query => query.Include(t => t.Bullets));
                if (task == null)
                {
                    throw new InvalidOperationException("Task not found.");
                }

                task.Name = model.Name;
                task.Description = model.Description;
                task.Progress = NormalizeProgress(model.Progress);
                task.TaskDate = model.TaskDate;
                task.Status = task.Status == TaskStatusEnum.Canceled
                    ? TaskStatusEnum.Canceled
                    : ResolveStatus(model.Progress);

                task.Bullets.Clear();
                foreach (var bullet in BuildBullets(model.Bullets))
                {
                    task.Bullets.Add(bullet);
                }

                await _taskRepository.UpdateAsync(task);
            });
        }

        public async Task UpdateProgressAsync(string id, int progress)
        {
            var task = await _taskRepository.GetByIdAsync(id);
            if (task == null)
            {
                throw new InvalidOperationException("Task not found.");
            }

            task.Progress = NormalizeProgress(progress);
            task.Status = ResolveStatus(task.Progress);

            await _taskRepository.UpdateAsync(task);
            await _taskRepository.SaveChangesAsync();
        }

        public async Task CancelAsync(string id)
        {
            var task = await _taskRepository.GetByIdAsync(id);
            if (task == null)
            {
                throw new InvalidOperationException("Task not found.");
            }

            task.Status = TaskStatusEnum.Canceled;
            await _taskRepository.UpdateAsync(task);
            await _taskRepository.SaveChangesAsync();
        }

        public async Task UncancelAsync(string id)
        {
            var task = await _taskRepository.GetByIdAsync(id);
            if (task == null)
            {
                throw new InvalidOperationException("Task not found.");
            }

            task.Status = ResolveStatus(task.Progress);
            await _taskRepository.UpdateAsync(task);
            await _taskRepository.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var task = await _taskRepository.GetByIdAsync(id);
            if (task == null)
            {
                return;
            }

            await _taskRepository.RemoveAsync(task);
            await _taskRepository.SaveChangesAsync();
        }

        private static int NormalizeProgress(int progress) => Math.Clamp(progress, 0, 100);

        private static TaskStatusEnum ResolveStatus(int progress)
        {
            var normalizedProgress = NormalizeProgress(progress);
            if (normalizedProgress == 100)
            {
                return TaskStatusEnum.Completed;
            }

            if (normalizedProgress > 0)
            {
                return TaskStatusEnum.InProgress;
            }

            return TaskStatusEnum.Pending;
        }

        private static List<TaskBullet> BuildBullets(IEnumerable<TaskBulletInputViewModel>? bullets)
        {
            return bullets?
                .Where(b => !string.IsNullOrWhiteSpace(b.Description))
                .Select(b => new TaskBullet
                {
                    Description = b.Description!.Trim(),
                    IsChecked = b.IsChecked
                })
                .ToList() ?? new List<TaskBullet>();
        }
    }
}
