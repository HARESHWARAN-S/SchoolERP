using SchoolERP.Models.Entities;

namespace SchoolERP.Repositories.Interfaces
{
    public interface IHomeworkRepository
    {
        Task<Homework?> GetAsync(int classId, string subject, DateOnly date);
        Task<List<Homework>> GetByClassIdAsync(int classId);
        Task AddAsync(Homework homework);
    }
}