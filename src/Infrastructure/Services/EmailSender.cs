using System.Net.Mail;
using System.Net;
using System.Threading.Tasks;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using Microsoft.eShopWeb.ApplicationCore;
using Microsoft.Extensions.Options;

namespace Microsoft.eShopWeb.Infrastructure.Services;

// This class is used by the application to send email for account confirmation and password reset.
// For more details see https://go.microsoft.com/fwlink/?LinkID=532713
public class EmailSender : IEmailSender
{
    private readonly EmailConfiguration _emailConfig;

    public EmailSender(IOptions<EmailConfiguration> emailConfig)
    {
        _emailConfig = emailConfig.Value;
    }

    public async Task SendEmailAsync(string email, string subject, string message)
    {
        MailMessage msg = new()
        {
            From = new MailAddress(_emailConfig.From!)
        };
        msg.To.Add(email);
        msg.Subject = subject;
        msg.Body = message;
        using SmtpClient smtp = new()
        {
            Host = _emailConfig.SmtpServer!,
            Port = _emailConfig.Port,
            EnableSsl = true,
            Credentials = new NetworkCredential(_emailConfig.From, _emailConfig.Password)
        };
        await smtp.SendMailAsync(msg);
    }
}
