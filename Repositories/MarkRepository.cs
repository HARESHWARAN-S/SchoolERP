using Microsoft.EntityFrameworkCore;
using SchoolERP.Contexts;
using SchoolERP.Models.Entities;
using SchoolERP.Repositories.Interfaces;

namespace SchoolERP.Repositories
{
    public class MarkRepository : IMarkRepository
    {
        private readonly SchoolERPDbContext _context;

        public MarkRepository(SchoolERPDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ExistsAsync(
            string examName, string subject, string Class, string sec)
        {
            return await _context.Marks
                .AnyAsync(m =>
                    m.ExamName == examName &&
                    m.Subject == subject &&
                    m.Class == Class &&
                    m.Sec == sec);
        }

        public async Task<List<Mark>> GetByStudentAsync(string admnNo)
        {
            return await _context.Marks
                .Where(m => m.AdmnNo == admnNo)
                .OrderByDescending(m => m.Date)
                .ToListAsync();
        }

        public async Task AddRangeAsync(List<Mark> marks)
        {
            await _context.Marks.AddRangeAsync(marks);
            await _context.SaveChangesAsync();
        }
    }
}