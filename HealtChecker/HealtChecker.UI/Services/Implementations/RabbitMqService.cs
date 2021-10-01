using HealtChecker.Shared.Models;
using HealtChecker.UI.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Diagnostics;
using System.Text;

namespace HealtChecker.UI.Services.Implementations
{
    public class RabbitMqService : IRabbitMqService
    {
        private IModel _logChannel { get; init; }
        private string _logQueueName { get; init; }

        public RabbitMqService(IConfiguration configuration)
        {
            try
            {
                ConnectionFactory connectionFactory = new ConnectionFactory()
                {
                    HostName = configuration["RabbitMq.HostName"]
                };
                IConnection connection = connectionFactory.CreateConnection();

                _logChannel = connection.CreateModel();

                _logQueueName = configuration["RabbitMq.LoggingQueue"];

                _logChannel.QueueDeclare(_logQueueName, false, false, false, null);
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);
                Trace.TraceError(ex.StackTrace);
            }

        }

        public void PushLog(LogItem log)
        {
            if (_logChannel == null)
            {
                Trace.TraceError(JsonConvert.SerializeObject(log));
                return;
            }
            try
            {
                _logChannel?.BasicPublish(string.Empty, _logQueueName, null,
                    Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(log)));
            }
            catch (Exception ex)
            {
                Trace.TraceError(JsonConvert.SerializeObject(log));
                Trace.TraceError(ex.Message);
                Trace.TraceError(ex.StackTrace);
            }
        }
    }

}
