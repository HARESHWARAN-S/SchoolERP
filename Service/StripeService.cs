using SchoolERP.Services.Interfaces;

namespace SchoolERP.Services
{
    public class StripeService : IStripeService
    {
        public async Task<string> MockChargeAsync(decimal amount, string admnNo, int feeId)
        {
            await Task.Delay(100); 
            string mockStripeId = $"stripe_mock_{admnNo}_{feeId}_{Guid.NewGuid().ToString("N")[..8]}";
            return mockStripeId;
        }
    }
}