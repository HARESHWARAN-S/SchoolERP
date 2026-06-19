using SchoolERP.Models.Entities;

namespace SchoolERP.Repositories.Interfaces
{
    public interface ISubjectRepository
    {
        Task<Subject?> GetAsync(string Class, string sec, string subjectName);
        Task<List<Subject>> GetAllAsync();
        Task<List<Subject>> GetByClassAsync(string Class, string sec);
        Task AddAsync(Subject subject);
        Task<Subject?> GetByClassSecSubjectTeacherAsync(string Class, string sec, string subject, string teacherId);
        Task<Subject?> GetByTeacherAsync(string Class, string sec, string teacherId);
        
    }
}