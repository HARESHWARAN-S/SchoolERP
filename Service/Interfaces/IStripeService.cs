namespace SchoolERP.Services.Interfaces
{
    public interface IStripeService
    {
        Task<string> MockChargeAsync(decimal amount, string admnNo, int feeId);
    }
}