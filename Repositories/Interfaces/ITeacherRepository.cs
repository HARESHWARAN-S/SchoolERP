using SchoolERP.Models.Entities;

namespace SchoolERP.Repositories.Interfaces
{
    public interface ITeacherRepository
    {
        Task<Teacher?> GetByIdAsync(string teacherId);
        Task<List<Teacher>> GetAllAsync();
        Task<string> GetNextTeacherIdAsync();
        Task AddAsync(Teacher teacher);
        Task UpdateAsync(Teacher teacher);
    }
}