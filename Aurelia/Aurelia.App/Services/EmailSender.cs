using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net;
using System.Net.Mail;

namespace Aurelia.App.Services
{
    public class EmailSender : IEmailSender
    {
        private string host;
        private int port;
        private bool enableSSL;
        private string username;
        private string password;
        
        // Get our parameterized configuration
        public EmailSender(string host, int port, bool enableSSL, string username, string password) {
            this.host = host;
            this.port = port;
            this.enableSSL = enableSSL;
            this.username = username;
            this.password = password;
        }
        
        // Use our configuration to send the email by using SmtpClient
        public Task SendEmailAsync(string email, string subject, string htmlMessage) {
            var client = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(username, password),
                EnableSsl = true,
            };
            return client.SendMailAsync(
                new MailMessage(username, email, subject, htmlMessage) { IsBodyHtml = true }
            );
        }
    }
}
