using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace happinesCafe.Services
{
    public class EmailService : IEmailService
    {
        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var mail = "reemalthibani46@gmail.com"; // البريد الإلكتروني المرسل
            var pw = "xirj jdns wmiz qxut"; // كلمة مرور البريد الإلكتروني

            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(mail, pw)
            };

            var mailMessage = new MailMessage(from: mail, to: email)
            {
                Subject = subject,
                Body = message,
                IsBodyHtml = true
            };

            await client.SendMailAsync(mailMessage);
        }
    }
}
