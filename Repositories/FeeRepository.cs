using Microsoft.EntityFrameworkCore;
using SchoolERP.Contexts;
using SchoolERP.Models.Entities;
using SchoolERP.Models.Enums;
using SchoolERP.Repositories.Interfaces;

namespace SchoolERP.Repositories
{
    public class FeeRepository : IFeeRepository
    {
        private readonly SchoolERPDbContext _context;

        public FeeRepository(SchoolERPDbContext context)
        {
            _context = context;
        }

        public async Task<Fee?> GetByIdAsync(int feeId)
        {
            return await _context.Fees
                .FirstOrDefaultAsync(f => f.FeeId == feeId);
        }

        public async Task<List<Fee>> GetDueByStudentAsync(string admnNo)
        {
            return await _context.Fees
                .Where(f => f.AdmnNo == admnNo && f.Status == FeeStatus.Unpaid)
                .OrderBy(f => f.DueDate)
                .ToListAsync();
        }

        public async Task AddRangeAsync(List<Fee> fees)
        {
            await _context.Fees.AddRangeAsync(fees);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Fee fee)
        {
            _context.Fees.Update(fee);
            await _context.SaveChangesAsync();
        }
    }
}