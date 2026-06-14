using Microsoft.EntityFrameworkCore;
using SchoolERP.Contexts;
using SchoolERP.Models.Entities;
using SchoolERP.Repositories.Interfaces;

namespace SchoolERP.Repositories
{
    public class SubjectRepository : ISubjectRepository
    {
        private readonly SchoolERPDbContext _context;

        public SubjectRepository(SchoolERPDbContext context)
        {
            _context = context;
        }

        public async Task<Subject?> GetAsync(string Class, string sec, string subjectName)
        {
            return await _context.Subjects
                .Include(s => s.Teacher)
                .FirstOrDefaultAsync(s =>
                    s.Class == Class &&
                    s.Sec == sec &&
                    s.SubjectName == subjectName);
        }

        public async Task<Subject?> GetByTeacherAsync(string Class, string sec, string teacherId)
        {
            return await _context.Subjects
                .Include(s => s.Teacher)
                .FirstOrDefaultAsync(s =>
                    s.Class == Class &&
                    s.Sec == sec );
        }

        public async Task<List<Subject>> GetAllAsync()
        {
            return await _context.Subjects
                .Include(s => s.Teacher)
                .Include(s => s.StudentClass)
                .OrderBy(s => s.Class)
                .ThenBy(s => s.Sec)
                .ThenBy(s => s.SubjectName)
                .ToListAsync();
        }

        public async Task<List<Subject>> GetByClassAsync(string Class, string sec)
        {
            return await _context.Subjects
                .Include(s => s.Teacher)
                .Where(s => s.Class == Class && s.Sec == sec)
                .OrderBy(s => s.SubjectName)
                .ToListAsync();
        }

        public async Task AddAsync(Subject subject)
        {
            await _context.Subjects.AddAsync(subject);
            await _context.SaveChangesAsync();
        }
        public async Task<Subject?> GetByClassSecSubjectTeacherAsync(
            string Class, string sec, string subject, string teacherId)
        {
            return await _context.Subjects
                .FirstOrDefaultAsync(s =>
                    s.Class == Class &&
                    s.Sec == sec &&
                    s.SubjectName == subject &&
                    s.TeacherId == teacherId);
        }
    }
}