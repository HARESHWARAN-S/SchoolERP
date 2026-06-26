using SchoolERP.Models.Entities;
using SchoolERP.Models.Enums;

namespace SchoolERP.Repositories.Interfaces
{
    public interface ILoginRepository
    {
        Task<Login?> GetByUsernameAsync(string username);
        Task<UserStatus?> GetStatusAsync(string username);
        Task AddAsync(Login login);
        Task UpdateAsync(Login login);
        Task UpdateResetCodeAsync(string username, string? code, DateTime? expiry);
    }
}