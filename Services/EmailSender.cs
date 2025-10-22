using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace DoAnLTW.Services
{
    public class EmailSender
    {
        private readonly string smtpServer = "smtp.gmail.com";
        private readonly int smtpPort = 587;
        private readonly string smtpUser = "thanhpham.apr@gmail.com";
        private readonly string smtpPass = "pwsomdxfjffvagxu";

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var client = new SmtpClient(smtpServer, smtpPort)
            {
                Credentials = new NetworkCredential(smtpUser, smtpPass),
                EnableSsl = true
            };
            var mail = new MailMessage(smtpUser, toEmail, subject, body)
            {
                IsBodyHtml = true
            };
            await client.SendMailAsync(mail);
        }
    }
} 