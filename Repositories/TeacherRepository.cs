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
    }
}