using BNS360.Core.Helpers.Settings;
using BNS360.Core.Services.Shared;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;


namespace BNS360.Reposatory.Repositories.Shared
{
    public class EmailService : IEmailService
    {
        private readonly MailSettings _mailSettings;

        public EmailService(IOptionsMonitor<MailSettings> options)
        {
            _mailSettings = options.CurrentValue;
        }

        public async Task SendEmailAsync(string toEmail,
            string subject, string body,
            CancellationToken cancellation = default)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_mailSettings.DisplayedName, _mailSettings.Email));
            message.To.Add(new MailboxAddress("", toEmail));
            message.Subject = subject;

            message.Body = new TextPart("html")
            {
                Text = body
            };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(_mailSettings.SmtpServer,_mailSettings.Port,
                    SecureSocketOptions.StartTls,cancellation);
                await client.AuthenticateAsync(_mailSettings.Email, _mailSettings.Password,cancellation);
                await client.SendAsync(message,cancellation);
                await client.DisconnectAsync(true, cancellation);
            }
        }
    }

}
