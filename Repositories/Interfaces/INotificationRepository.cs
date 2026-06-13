using SchoolERP.Models.Entities;
using SchoolERP.Models.Enums;

namespace SchoolERP.Repositories.Interfaces
{
    public interface INotificationRepository
    {
        Task AddAsync(Notification notification);
        Task<List<Notification>> GetAllAsync();
        Task<List<Notification>> GetByTargetAsync(NotificationTarget target);
    }
}