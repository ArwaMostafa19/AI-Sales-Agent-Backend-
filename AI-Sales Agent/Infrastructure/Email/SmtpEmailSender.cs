using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;

namespace AI_Sales_Agent.Infrastructure.Email
{
    public interface IEmailSender
    {
        Task SendAsync(string to, string subject, string htmlBody, CancellationToken cancellationToken);
    }

    public class SmtpEmailSender : IEmailSender
    {
        private readonly EmailOptions _options;
        private readonly ILogger<SmtpEmailSender> _logger;

        public SmtpEmailSender(IOptions<EmailOptions> options, ILogger<SmtpEmailSender> logger)
        {
            _options = options.Value;
            _logger = logger;
        }

        public async Task SendAsync(string to, string subject, string htmlBody, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(_options.Host)
                || string.IsNullOrWhiteSpace(_options.From)
                || (!string.IsNullOrWhiteSpace(_options.UserName) && string.IsNullOrWhiteSpace(_options.Password)))
            {
                _logger.LogInformation(
                    "Email is not configured completely. Message to {Email}. Subject: {Subject}. Body: {Body}",
                    to,
                    subject,
                    htmlBody);
                return;
            }

            using var message = new MailMessage
            {
                From = new MailAddress(_options.From, _options.DisplayName),
                Subject = subject,
                Body = htmlBody,
                IsBodyHtml = true
            };
            message.To.Add(to);

            using var client = new SmtpClient(_options.Host, _options.Port)
            {
                EnableSsl = _options.EnableSsl
            };

            if (!string.IsNullOrWhiteSpace(_options.UserName))
            {
                client.Credentials = new NetworkCredential(_options.UserName, _options.Password);
            }

            try
            {
                await client.SendMailAsync(message, cancellationToken);
            }
            catch (Exception exception)
            {
                _logger.LogError(
                    exception,
                    "Email sending failed. Message to {Email}. Subject: {Subject}. Body: {Body}",
                    to,
                    subject,
                    htmlBody);
            }
        }
    }
}
