using SchoolERP.Models.Entities;

namespace SchoolERP.Repositories.Interfaces
{
    public interface ISubjectRepository
    {
        Task<Subject?> GetAsync(int classId, string subjectName);
        Task<List<Subject>> GetAllAsync();
        Task<List<Subject>> GetByClassAsync(int classId);
        Task AddAsync(Subject subject);
        Task<Subject?> GetByClassSecSubjectTeacherAsync(int classId, string subject, string teacherId);
        Task<Subject?> GetByTeacherAsync(int classId, string teacherId);
        Task<List<Subject>> GetByClassIdAsync(int classId);
        
    }
}