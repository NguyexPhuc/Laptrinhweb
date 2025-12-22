using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Web_BanHang.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _cfg;
        public EmailSender(IConfiguration cfg)
        {
            _cfg = cfg;
        }

        public async Task SendEmailAsync(string to, string subject, string htmlMessage)
        {
            var smtp = _cfg.GetSection("Smtp");
            var host = smtp.GetValue<string>("Host");
            if (string.IsNullOrEmpty(host))
            {
                // Fallback: write to file for development
                var pickup = smtp.GetValue<string>("PickupDirectory") ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Web_BanHang", "Emails");
                Directory.CreateDirectory(pickup);
                var file = Path.Combine(pickup, $"email_{DateTime.UtcNow:yyyyMMddHHmmssfff}.html");
                await File.WriteAllTextAsync(file, $"To: {to}\nSubject: {subject}\n\n{htmlMessage}");
                return;
            }

            var port = smtp.GetValue<int>("Port");
            var enableSsl = smtp.GetValue<bool>("EnableSsl");
            var user = smtp.GetValue<string>("User");
            var pass = smtp.GetValue<string>("Password");
            var from = smtp.GetValue<string>("From") ?? user ?? "no-reply@example.com";

            using var msg = new MailMessage(from, to, subject, htmlMessage);
            msg.IsBodyHtml = true;

            using var client = new SmtpClient(host, port)
            {
                EnableSsl = enableSsl
            };
            if (!string.IsNullOrEmpty(user)) client.Credentials = new NetworkCredential(user, pass);
            await client.SendMailAsync(msg);
        }
    }
}