using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using MimeKit;
using SurveyBasket.Api.Models;

namespace SurveyBasket.Api.Services
{
    public class EmailService(IOptions<MailSettings> mailsettings) : IEmailSender
    {
        private readonly MailSettings Mailsettings = mailsettings.Value;

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var message = new MimeMessage
            {
                Sender = MailboxAddress.Parse(Mailsettings.Mail),
                Subject = subject,

            };
            message.To.Add(MailboxAddress.Parse(email));
            var builder = new BodyBuilder
            {
                HtmlBody = htmlMessage
            };
            message.Body = builder.ToMessageBody();
            using var smtp = new SmtpClient();
            smtp.Connect(Mailsettings.Host, Mailsettings.Port,SecureSocketOptions.StartTls);
            smtp.Authenticate(Mailsettings.Mail, Mailsettings.Password);
            await smtp.SendAsync(message);
            smtp.Disconnect(true);
        }
    }
}
