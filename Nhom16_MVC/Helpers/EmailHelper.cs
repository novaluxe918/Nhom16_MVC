using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace Nhom16_MVC.Helpers
{
    public class EmailHelper
    {
        private readonly IConfiguration _configuration;

        public EmailHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var smtpServer = _configuration["EmailSettings:SmtpServer"];
            var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"] ?? "587");
            var senderEmail = _configuration["EmailSettings:SenderEmail"];
            var senderName = _configuration["EmailSettings:SenderName"];
            var senderPassword = _configuration["EmailSettings:SenderPassword"];
            var enableSsl = bool.Parse(_configuration["EmailSettings:EnableSsl"] ?? "true");

            using var message = new MailMessage();
            message.From = new MailAddress(senderEmail!, senderName);
            message.To.Add(new MailAddress(toEmail));
            message.Subject = subject;
            message.Body = body;
            message.IsBodyHtml = true;

            using var client = new SmtpClient(smtpServer, smtpPort);
            client.Credentials = new NetworkCredential(senderEmail, senderPassword);
            client.EnableSsl = enableSsl;

            await client.SendMailAsync(message);
        }
    }
}