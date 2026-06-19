using Microsoft.EntityFrameworkCore;
using SchoolERP.Contexts;
using SchoolERP.Models.Entities;
using SchoolERP.Models.Enums;
using SchoolERP.Repositories.Interfaces;

namespace SchoolERP.Repositories
{
    public class TeacherRepository : ITeacherRepository
    {
        private readonly SchoolERPDbContext _context;

        public TeacherRepository(SchoolERPDbContext context)
        {
            _context = context;
        }

        public async Task<Teacher?> GetByIdAsync(string teacherId)
        {
            return await _context.Teachers
                .FirstOrDefaultAsync(t => t.TeacherId == teacherId);
        }

        public async Task<List<Teacher>> GetAllAsync()
        {
            return await _context.Teachers.ToListAsync();
        }

        public async Task<string> GetNextTeacherIdAsync()
        {
            var teachers = await _context.Teachers.ToListAsync();
            if (!teachers.Any())
                return "T1";

            int maxId = teachers
                .Select(t => t.TeacherId.Replace("T", ""))
                .Where(t => int.TryParse(t, out _))
                .Select(t => int.Parse(t))
                .Max();

            return "T" + (maxId + 1).ToString();
        }

        public async Task AddAsync(Teacher teacher)
        {
            await _context.Teachers.AddAsync(teacher);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Teacher teacher)
        {
            _context.Teachers.Update(teacher);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Teacher>> GetActiveTeachersAsync()
        {
            return await _context.Teachers
                .Join(_context.Logins,
                    t => t.TeacherId,
                    l => l.Username,
                    (t, l) => new { Teacher = t, Login = l })
                .Where(x => x.Login.Status == UserStatus.Active)
                .Select(x => x.Teacher)
                .ToListAsync();
        }

        public async Task<bool> ExistsByTimetableAsync(string timetableUrl)
        {
            return await _context.Teachers
                .AnyAsync(t => t.TimeTableUrl == timetableUrl);
        }

        public async Task<bool> ExistsActiveByContactNoAsync(string contactNo)
        {
            return await _context.Teachers
                .Join(_context.Logins,
                    t => t.TeacherId,
                    l => l.Username,
                    (t, l) => new { Teacher = t, Login = l })
                .AnyAsync(x => x.Teacher.ContactNo == contactNo && x.Login.Status == UserStatus.Active);
        }
    }
}