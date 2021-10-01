using HealtChecker.Service.Metrics.Services.Interfaces;
using HealtChecker.Shared.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace HealtChecker.Service.Metrics.Services.Implementations
{
    public class EmailNotificationService : IEmailNotificationService
    {
        private IConfiguration _configuration { get; set; }
        private IRabbitMqService _rabbitMqService { get; set; }
        private string FromEmail { get; init; }
        private string Host { get; init; }
        private int Port { get; init; }
        private string UserName { get; init; }
        private string Password { get; init; }
        public EmailNotificationService(IConfiguration configuration, IRabbitMqService rabbitMqService)
        {
            _configuration = configuration;
            _rabbitMqService = rabbitMqService;
            FromEmail = _configuration["EmailSettings.From"];
            Host = _configuration["EmailSettings.Host"];
            Int32.TryParse(_configuration["EmailSettings.Port"], out int port);
            Port = port;
            UserName = _configuration["EmailSettings.UserName"];
            Password = _configuration["EmailSettings.Password"];
        }

        public void SendEmail(MetricItem metricItem)
        {

            var mailMessage = new MailMessage(new MailAddress(FromEmail), new MailAddress(metricItem.NotificationEmailAddress))
            {
                IsBodyHtml = true,
                Subject = $"Healt Check Alert {metricItem.Name}",
                Body = JsonConvert.SerializeObject(metricItem)
            };

            var smtp = new SmtpClient
            {
                Host = Host,
                Port = Port,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(UserName, Password)
            };

            try
            {
                smtp.Send(mailMessage);
            }
            catch (Exception ex)
            {
                LogItem logItem = LogItem.CreateLogItemFromException(ex, Channel.ServiceMetrics);
                _rabbitMqService.PushLog(logItem);
            }
        }
    }
}
