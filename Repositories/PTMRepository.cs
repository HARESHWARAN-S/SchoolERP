using Microsoft.EntityFrameworkCore;
using SchoolERP.Contexts;
using SchoolERP.Models.Entities;
using SchoolERP.Repositories.Interfaces;

namespace SchoolERP.Repositories
{
    public class PTMRepository : IPTMRepository
    {
        private readonly SchoolERPDbContext _context;

        public PTMRepository(SchoolERPDbContext context)
        {
            _context = context;
        }

        public async Task<PTM?> GetAsync(string admnNo, string teacherId, DateOnly date)
        {
            return await _context.PTMs
                .FirstOrDefaultAsync(p =>
                    p.AdmnNo == admnNo &&
                    p.TeacherId == teacherId &&
                    p.Date == date);
        }

        public async Task<List<PTM>> GetByStudentAndClassIdAsync(string admnNo, int classId)
        {
            return await _context.PTMs
                .Include(p => p.Teacher)
                .Include(p => p.StudentClass)
                .Where(p => p.AdmnNo == admnNo && p.ClassId == classId)
                .OrderByDescending(p => p.Date)
                .ToListAsync();
        }

        public async Task AddAsync(PTM ptm)
        {
            await _context.PTMs.AddAsync(ptm);
            await _context.SaveChangesAsync();
        }
    }
}