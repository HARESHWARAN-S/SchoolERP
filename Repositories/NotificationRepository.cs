using Microsoft.EntityFrameworkCore;
using SchoolERP.Contexts;
using SchoolERP.Models.Entities;
using SchoolERP.Models.Enums;
using SchoolERP.Repositories.Interfaces;

namespace SchoolERP.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly SchoolERPDbContext _context;

        public NotificationRepository(SchoolERPDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Notification notification)
        {
            await _context.Notifications.AddAsync(notification);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Notification>> GetAllAsync()
        {
            return await _context.Notifications
                .OrderByDescending(n => n.Timestamp)
                .ToListAsync();
        }

        public async Task<List<Notification>> GetByTargetAsync(NotificationTarget target)
        {
            return await _context.Notifications
                .Where(n => n.Target == target || n.Target == NotificationTarget.All)
                .OrderByDescending(n => n.Timestamp)
                .ToListAsync();
        }
    }
}