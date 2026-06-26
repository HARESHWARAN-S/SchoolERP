using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using SchoolERP.Services.Interfaces;

namespace SchoolERP.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendResetCodeAsync(string toEmail, string code, string username)
        {
            var host = _config["Smtp:Host"]!;
            var port = int.Parse(_config["Smtp:Port"]!);
            var smtpUsername = _config["Smtp:Username"]!;
            var smtpPassword = _config["Smtp:Password"]!;
            var fromName = _config["Smtp:FromName"]!;

            using var client = new SmtpClient(host, port)
            {
                Credentials = new NetworkCredential(smtpUsername, smtpPassword),
                EnableSsl = true
            };

            var mail = new MailMessage
            {
                From = new MailAddress(smtpUsername, fromName),
                Subject = "SchoolERP — Password Reset Code",
                Body = $@"
Hello {username},

Your password reset code is:

    {code}

This code expires in 5 minutes.
If you did not request this, please ignore this email.

Regards,
SchoolERP Team
                ",
                IsBodyHtml = false
            };
            mail.To.Add(toEmail);

            await client.SendMailAsync(mail);
        }
    }
}