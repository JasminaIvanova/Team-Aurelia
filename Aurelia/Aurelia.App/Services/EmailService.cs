using MimeKit;
using Aurelia.App.Models;
using MailKit.Net.Smtp;

namespace Aurelia.App.Services
{
    public class EmailService : IEmailService
    {
        private readonly GmailConf gmailConfiguration;

        public EmailService(GmailConf gmailConfiguration)
        {
            this.gmailConfiguration = gmailConfiguration;
        }

        private MimeMessage CreateEmailMessage(Message message)
        {
            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress(gmailConfiguration.From));
            emailMessage.To.AddRange(message.To);
            emailMessage.Subject = message.Subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Text) { Text = message.Content };

            return emailMessage;
        }

        private void Send(MimeMessage mimeMessage)
        {
            using (var client = new SmtpClient())
            {
                try
                {
                    client.Connect(this.gmailConfiguration.SmtpServer, this.gmailConfiguration.Port, true);
                    client.Authenticate(this.gmailConfiguration.UserName, this.gmailConfiguration.Password);
                    client.Send(mimeMessage);
                }
                catch (Exception ignored)
                {
                    Console.WriteLine(ignored);
                }
                finally
                {
                    client.Disconnect(true);
                    client.Dispose();
                }
            }
        }

        public void SendEmail(Message message)
        {
            this.Send(this.CreateEmailMessage(message));
        }
    }
 }
