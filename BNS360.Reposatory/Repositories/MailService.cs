using BNS360.Core.Services;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNS360.Reposatory.Repositories
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;        
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_config["mail_settings:displayed_name"], _config["mail_settings:from"]));
            message.To.Add(new MailboxAddress("", toEmail));
            message.Subject = subject;

            message.Body = new TextPart("html")
            {
                Text = body
            };
            
            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(_config["mail_settings:smtp_server"],
                    int.Parse(_config["mail_settings:port"] ?? throw new InvalidOperationException("port is null")),
                    SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_config["mail_settings:from"], _config["mail_settings:password"]);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
        }
    }

}
