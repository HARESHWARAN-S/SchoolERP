using SchoolERP.Models.Entities;

namespace SchoolERP.Repositories.Interfaces
{
    public interface IAdminRepository
    {
        Task<Admin?> GetByIdAsync(string adminId);
        Task<Admin?> GetActiveAdminAsync();
        Task<int> GetAdminCountAsync();
        Task<string> GetNextAdminIdAsync();
        Task AddAsync(Admin admin);
        Task UpdateAsync(Admin admin);
    }
}