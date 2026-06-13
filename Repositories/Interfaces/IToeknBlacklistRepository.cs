using SchoolERP.Models.Entities;

namespace SchoolERP.Repositories.Interfaces
{
    public interface ITokenBlacklistRepository
    {
        Task AddAsync(BlacklistedToken token);
        Task<bool> IsBlacklistedAsync(string token);
    }
}