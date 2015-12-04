using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.OptionsModel;

namespace DBC.Services
{
    // This class is used by the application to send Email and SMS when you turn on two-factor authentication in ASP.NET Identity.
    // For more details see this link http://go.microsoft.com/fwlink/?LinkID=532713

    public class MessageServices : IEmailSender, ISmsSender
    {
        private readonly MessageServicesOptions _settings;

        public MessageServices(IOptions<MessageServicesOptions> settingsAccessor)
        {
            _settings = settingsAccessor.Value;
        }

        public Task SendEmailAsync(string email, string subject, string message)
        {
            return SendEmailAsync(email, subject, message, null, null);
        }

        public Task SendSmsAsync(string number, string message)
        {
            // Plug in your SMS service here to send a text message.
            return Task.FromResult(0);
        }

        public Task SendEmailAsync(string email, string subject, string message, string from, string fromName)
        {
            var mailMessage = new MailMessage
            {
                From = new MailAddress(@from ?? _settings.@from, fromName ?? _settings.fromName),
                Body = message,
                IsBodyHtml = true,
                Subject = subject
            };
            mailMessage.To.Add(email);
            using (var smtpClient = new SmtpClient())
            {
                smtpClient.Host = _settings.host;
                smtpClient.Port = _settings.port;
                smtpClient.EnableSsl = _settings.enableSsl;
                smtpClient.DeliveryMethod = _settings.deliveryMethod == "SpecifiedPickupDirectory"
                    ? SmtpDeliveryMethod.SpecifiedPickupDirectory
                    : SmtpDeliveryMethod.Network;
                smtpClient.PickupDirectoryLocation = _settings.pickupDirectoryLocation;
                smtpClient.Credentials = new NetworkCredential(_settings.userName, _settings.password);
                return smtpClient.SendMailAsync(mailMessage);
            }
        }
    }
}