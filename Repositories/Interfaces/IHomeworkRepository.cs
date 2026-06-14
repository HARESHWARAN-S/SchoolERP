using SchoolERP.Models.Entities;

namespace SchoolERP.Repositories.Interfaces
{
    public interface IHomeworkRepository
    {
        Task<Homework?> GetAsync(string Class, string sec, string subject, DateOnly date);
        Task<List<Homework>> GetByClassAsync(string Class, string sec);
        Task AddAsync(Homework homework);
    }
}