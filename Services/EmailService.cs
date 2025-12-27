using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;

namespace LearningPlatform.Services
{
    public interface IEmailService
    {
        void SendEmail(string to, string subject, string body);
    }

    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public void SendEmail(string to, string subject, string body)
        {
            var myEmail = _config["EmailSettings:FromEmail"]; 
            var myPassword = _config["EmailSettings:Password"];
            var myHost = _config["EmailSettings:Host"];
            var portString = _config["EmailSettings:Port"];

            if (string.IsNullOrEmpty(myEmail)) throw new Exception("JSON Error: 'FromEmail' is missing.");
            if (string.IsNullOrEmpty(myPassword)) throw new Exception("JSON Error: 'Password' is missing.");

            int myPort = int.Parse(portString ?? "587");

            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(myEmail));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;
            email.Body = new TextPart(TextFormat.Html) { Text = body };

            using var smtp = new SmtpClient();
            smtp.Connect(myHost, myPort, SecureSocketOptions.StartTls);
            smtp.Authenticate(myEmail, myPassword);
            smtp.Send(email);
            smtp.Disconnect(true);
        }
    }
}
