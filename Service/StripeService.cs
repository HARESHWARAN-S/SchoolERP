using SchoolERP.Services.Interfaces;

namespace SchoolERP.Services
{
    public class StripeService : IStripeService
    {
        public async Task<string> MockChargeAsync(decimal amount, string admnNo, int feeId)
        {
            // Mock Stripe payment — generates a fake payment ID
            await Task.Delay(100); // simulate API call delay
            string mockStripeId = $"stripe_mock_{admnNo}_{feeId}_{Guid.NewGuid().ToString("N")[..8]}";
            return mockStripeId;
        }
    }
}