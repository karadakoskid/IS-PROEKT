using EShop.Domain.Email;
using EShop.Service.Interface;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace EShop.Service.Implementation
{
    public class EmailService : IEmailService
    {
        private readonly MailSettings _mailSettings;

        public EmailService(IOptions<MailSettings> mailSettings)
        {
            _mailSettings = mailSettings.Value;
        }


        public async Task SendEmailAsync(EmailMessage message)
        {
            var email = new MimeMessage
            {
                Sender = new MailboxAddress(_mailSettings.SendersName, _mailSettings.SmtpUserName),
                Subject = message.Subject
            };

            email.From.Add(new MailboxAddress(_mailSettings.SendersName, _mailSettings.SmtpUserName));
            email.To.Add(new MailboxAddress(message.MailTo, message.MailTo));
            email.Body = new TextPart(MimeKit.Text.TextFormat.Plain) { Text = message.Content };

            try
            {
                using (var smtp = new MailKit.Net.Smtp.SmtpClient())
                {
                    var socketOptions = SecureSocketOptions.Auto;

                    await smtp.ConnectAsync(_mailSettings.SmtpServer, 587, socketOptions);

                    if (!string.IsNullOrEmpty(_mailSettings.SmtpUserName))
                    {
                        await smtp.AuthenticateAsync(_mailSettings.SmtpUserName, _mailSettings.SmtpPassword);
                    }
                    await smtp.SendAsync(email);


                    await smtp.DisconnectAsync(true);
                }

            }
            catch (SmtpException ex)
            {
                throw ex;
            }
        }
    }
}
