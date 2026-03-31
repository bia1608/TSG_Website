using MailKit.Security;
using MailKit.Net.Smtp;
using MimeKit;

namespace TSG_Website.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        private async Task SendAsync(string toEmail, string toName, string subject, string htmlBody)
        {
            var msg = new MimeMessage();
            msg.From.Add(new MailboxAddress("Echipa TSG", "noreply@tsg.ro"));
            msg.To.Add(new MailboxAddress(toName, toEmail));
            msg.Subject = subject;
            msg.Body = new BodyBuilder { HtmlBody = htmlBody }.ToMessageBody();

            using var smtp = new MailKit.Net.Smtp.SmtpClient();
            await smtp.ConnectAsync(
                _config["Email:Smtp:Host"], 
                int.Parse(_config["Email:Smtp:Port"]), 
                SecureSocketOptions.None
            );
            await smtp.SendAsync(msg);
            await smtp.DisconnectAsync(true);
        }
        

        public async Task SendWelcomeEmailAsync(string email, string firstName, string password)
        {
            var html = $@"
                <p>Bine ai venit, {firstName}!</p>
                <p>Cererea ta a fost acceptată. Datele tale de acces:</p>
            <p><strong>Email:</strong> {email}</p>
            <p><strong>Parolă:</strong> {password}</p>
            <p style=""color:red"">Schimbă parola la prima autentificare!</p>
            ";
            await SendAsync(email, firstName, "Cont creat - TSG", html);
        }

        public async Task SendRegistrationConfirmationAsync(string email, string name)
        {
            var html = $@"
                <p>Bună, {name}!</p>
                <p>Am primit cererea ta de înregistrare. Te vom anunța în curând dacă a fost acceptată.</p>
            ";
            await SendAsync(email, name, "Cerere de înregistrare primită - TSG", html);
        }

        public async Task SendRegistrationRejectedAsync(string email, string name, string? reason)
        {
            var html = $@"
                <p>Bună, {name}!</p>
                <p>Ne pare rău să te informăm că cererea ta de înregistrare a fost respinsă.</p>
                {(string.IsNullOrEmpty(reason) ? "" : $"<p>Motiv: {reason}</p>")}
            ";
            await SendAsync(email, name, "Cerere de înregistrare respinsă - TSG", html);
        }
    }
}