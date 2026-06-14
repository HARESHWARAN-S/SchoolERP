using Microsoft.EntityFrameworkCore;
using SchoolERP.Contexts;
using SchoolERP.Models.Entities;
using SchoolERP.Repositories.Interfaces;

namespace SchoolERP.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly SchoolERPDbContext _context;

        public PaymentRepository(SchoolERPDbContext context)
        {
            _context = context;
        }

        public async Task<Payment> AddAsync(Payment payment)
        {
            await _context.Payments.AddAsync(payment);
            await _context.SaveChangesAsync();
            return payment;
        }

        public async Task<List<Payment>> GetByStudentAsync(string admnNo)
        {
            return await _context.Payments
                .Where(p => p.AdmnNo == admnNo)
                .OrderByDescending(p => p.Date)
                .ToListAsync();
        }
    }
}