using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Framework.OptionsModel;

namespace DBC.Services
{
    // This class is used by the application to send Email and SMS when you turn on two-factor authentication in ASP.NET Identity.
    // For more details see this link http://go.microsoft.com/fwlink/?LinkID=532713

    public class MessageServices : IEmailSender, ISmsSender
    {
        public MessageServices(IOptions<MessageServicesOptions> settingsAccessor)
        {
            Setting = settingsAccessor.Value;
        }

        public MessageServicesOptions Setting { get; }

        public Task SendEmailAsync(string email, string subject, string message)
        {
            return SendEmailAsync(email, subject, message, null, null);
        }
        public Task SendEmailAsync(string email, string subject, string message, string from, string fromName)
        {
            var mailMessage = new MailMessage
            {
                From = new MailAddress(@from ?? Setting.@from, fromName ?? Setting.fromName),
                Body = message,
                IsBodyHtml = true,
                Subject = subject
            };
            mailMessage.To.Add(email);
            using (var smtpClient = new SmtpClient()) {
                smtpClient.Host = Setting.host;
                smtpClient.Port = Setting.port;
                smtpClient.EnableSsl = Setting.enableSsl;
                smtpClient.DeliveryMethod = Setting.deliveryMethod == "SpecifiedPickupDirectory"
                    ? SmtpDeliveryMethod.SpecifiedPickupDirectory
                    : SmtpDeliveryMethod.Network;
                smtpClient.PickupDirectoryLocation = Setting.pickupDirectoryLocation;
                smtpClient.Credentials = new NetworkCredential(Setting.userName, Setting.password);
                return smtpClient.SendMailAsync(mailMessage);
            }
        }

        public Task SendSmsAsync(string number, string message)
        {
            // Plug in your SMS service here to send a text message.
            return Task.FromResult(0);
        }


    }
}