using Microsoft.EntityFrameworkCore;
using SchoolERP.Contexts;
using SchoolERP.Models.Entities;
using SchoolERP.Repositories.Interfaces;

namespace SchoolERP.Repositories
{
    public class HomeworkRepository : IHomeworkRepository
    {
        private readonly SchoolERPDbContext _context;

        public HomeworkRepository(SchoolERPDbContext context)
        {
            _context = context;
        }

        public async Task<Homework?> GetAsync(int classId, string subject, DateOnly date)
        {
            return await _context.Homeworks
                .FirstOrDefaultAsync(h =>
                    h.ClassId == classId &&
                    h.Subject == subject &&
                    h.Date == date);
        }

        public async Task<List<Homework>> GetByClassIdAsync(int classId)
        {
            return await _context.Homeworks
                .Where(h => h.ClassId == classId)
                .OrderByDescending(h => h.Date)
                .ToListAsync();
        }

        public async Task AddAsync(Homework homework)
        {
            await _context.Homeworks.AddAsync(homework);
            await _context.SaveChangesAsync();
        }
    }
}