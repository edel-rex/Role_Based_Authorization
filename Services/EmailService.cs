using System;
using jwt_employee.Interface;
using jwt_employee.Models;
using System.Net.Mail;
using System.Net;

namespace AutoSend_Email.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;
        public EmailService(IConfiguration configuration)
        {
            _config = configuration;
        }
        public void SendEmail(Email request)
        {
            var email = new MailMessage();
            email.To.Add(request.To.Trim());
            email.From = new MailAddress(_config["Email:userID"], "No-reply");
            email.Subject = request.Subject;
            email.IsBodyHtml = true;
            email.Body = request.Body;
            email.Priority = MailPriority.Normal;

            SmtpClient smtp = new SmtpClient();
            smtp.Host = _config["Email:host"];
            smtp.Port = 587;
            smtp.EnableSsl = true;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential(_config["Email:userID"], _config["Email:password"]);
            smtp.Send(email);
        }
    }
}
