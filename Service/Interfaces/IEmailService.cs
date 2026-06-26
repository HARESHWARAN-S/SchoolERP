namespace SchoolERP.Services.Interfaces
{
    public interface IEmailService
    {
        Task SendResetCodeAsync(string toEmail, string code, string username);
    }
}