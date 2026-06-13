using Microsoft.EntityFrameworkCore;
using SchoolERP.Contexts;
using SchoolERP.Models.Entities;
using SchoolERP.Repositories.Interfaces;

namespace SchoolERP.Repositories
{
    public class TokenBlacklistRepository : ITokenBlacklistRepository
    {
        private readonly SchoolERPDbContext _context;

        public TokenBlacklistRepository(SchoolERPDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(BlacklistedToken token)
        {
            await _context.BlacklistedTokens.AddAsync(token);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsBlacklistedAsync(string token)
        {
            return await _context.BlacklistedTokens
                .AnyAsync(t => t.Token == token);
        }
    }
}