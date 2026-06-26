using Microsoft.EntityFrameworkCore;
using SchoolERP.Contexts;
using SchoolERP.Models.Entities;
using SchoolERP.Models.Enums;
using SchoolERP.Repositories.Interfaces;

namespace SchoolERP.Repositories
{
    public class LoginRepository : ILoginRepository
    {
        private readonly SchoolERPDbContext _context;

        public LoginRepository(SchoolERPDbContext context)
        {
            _context = context;
        }

        public async Task<Login?> GetByUsernameAsync(string username)
        {
            return await _context.Logins
                .FirstOrDefaultAsync(l => l.Username == username);
        }

        public async Task<UserStatus?> GetStatusAsync(string username)
        {
            var login = await _context.Logins
                .FirstOrDefaultAsync(l => l.Username == username);
            return login?.Status;
        }

        public async Task AddAsync(Login login)
        {
            await _context.Logins.AddAsync(login);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Login login)
        {
            _context.Logins.Update(login);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateResetCodeAsync(string username, string? code, DateTime? expiry)
        {
            var login = await _context.Logins
                .FirstOrDefaultAsync(l => l.Username == username);
            if (login != null)
            {
                login.ResetCode = code;
                login.ResetCodeExpiry = expiry;
                await _context.SaveChangesAsync();
            }
        }
    }
}