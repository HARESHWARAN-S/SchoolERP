using Microsoft.EntityFrameworkCore;
using SchoolERP.Contexts;
using SchoolERP.Models.Entities;
using SchoolERP.Models.Enums;
using SchoolERP.Repositories.Interfaces;

namespace SchoolERP.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        private readonly SchoolERPDbContext _context;

        public AdminRepository(SchoolERPDbContext context)
        {
            _context = context;
        }

        public async Task<Admin?> GetByIdAsync(string adminId)
        {
            return await _context.Admins
                .FirstOrDefaultAsync(a => a.AdminId == adminId);
        }

        public async Task<Admin?> GetActiveAdminAsync()
        {
            return await _context.Admins
                .Include(a => a.Login)
                .FirstOrDefaultAsync(a => a.Login.Status == UserStatus.Active);
        }

        public async Task<int> GetAdminCountAsync()
        {
            return await _context.Admins.CountAsync();
        }

        public async Task<string> GetNextAdminIdAsync()
        {
            int count = await _context.Admins.CountAsync();
            return "A" + (count + 1).ToString();
        }

        public async Task AddAsync(Admin admin)
        {
            await _context.Admins.AddAsync(admin);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Admin admin)
        {
            _context.Admins.Update(admin);
            await _context.SaveChangesAsync();
        }
    }
}