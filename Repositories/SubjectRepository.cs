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

        public async Task<Subject?> GetAsync(int classId, string subjectName)
        {
            return await _context.Subjects
                .Include(s => s.Teacher)
                .FirstOrDefaultAsync(s => s.ClassId == classId && s.SubjectName == subjectName);
        }

        public async Task<Subject?> GetByTeacherAsync(int classId, string teacherId)
        {
            return await _context.Subjects
                .Include(s => s.Teacher)
                .FirstOrDefaultAsync(s => s.ClassId == classId && s.TeacherId == teacherId);
        }

        public async Task<List<Subject>> GetAllAsync()
        {
            return await _context.Subjects
                .Include(s => s.Teacher)
                .Include(s => s.StudentClass)
                .OrderBy(s => s.StudentClass!.Class)
                .ThenBy(s => s.StudentClass!.Sec)
                .ThenBy(s => s.SubjectName)
                .ToListAsync();
        }

        public async Task<List<Subject>> GetByClassAsync(int classId)
        {
            return await _context.Subjects
                .Include(s => s.Teacher)
                .Where(s => s.ClassId == classId)
                .OrderBy(s => s.SubjectName)
                .ToListAsync();
        }

        public async Task AddAsync(Subject subject)
        {
            await _context.Subjects.AddAsync(subject);
            await _context.SaveChangesAsync();
        }
        public async Task<Subject?> GetByClassSecSubjectTeacherAsync(
            int classId, string subject, string teacherId)
        {
            return await _context.Subjects
                .FirstOrDefaultAsync(s =>
                    s.ClassId == classId &&
                    s.SubjectName == subject &&
                    s.TeacherId == teacherId);
        }

        public async Task<List<Subject>> GetByClassIdAsync(int classId)
        {
            return await _context.Subjects
                .Include(s => s.Teacher)
                .Where(s => s.ClassId == classId)
                .OrderBy(s => s.SubjectName)
                .ToListAsync();
        }
    }
}