using SchoolERP.Models.Entities;

namespace SchoolERP.Repositories.Interfaces
{
    public interface ILogRepository
    {
        Task AddAsync(string message);
    }
}