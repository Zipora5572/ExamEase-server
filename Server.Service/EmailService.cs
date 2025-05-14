using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using Server.Core.Entities;
using Server.Core.IServices;
using System;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using System.Net;

namespace Server.Service
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration configuration;

        public EmailService(IConfiguration _configuration)
        {
            configuration = _configuration;
        }

        public async Task<bool> SendEmailAsync(EmailRequest request)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("Exams-App", configuration["GOOGLE_USER_EMAIL"]));
            emailMessage.To.Add(new MailboxAddress(request.To, request.To));
            emailMessage.Subject = request.Subject;

            var bodyBuilder = new BodyBuilder { TextBody = request.Body };
            emailMessage.Body = bodyBuilder.ToMessageBody();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;
            using (var client = new SmtpClient())
            {
                try
                {
                    client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                    await client.ConnectAsync(configuration["EmailSettings:SmtpServer"],
                                              int.Parse(configuration["EmailSettings:Port"]),
                                              SecureSocketOptions.StartTls);

                   
                    await client.AuthenticateAsync(configuration["EmailSettings:Username"],
                                                    configuration["EmailSettings:Password"]);

                   
                    await client.SendAsync(emailMessage);
                    await client.DisconnectAsync(true);
                    return true;
                }
                catch (Exception ex)
                {
                   
                    Console.WriteLine($"Error sending email: {ex.Message}");
                    return false;
                }
            }
        }
    }
}
