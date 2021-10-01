using HealtChecker.Service.Logging.Services.Interfaces;
using HealtChecker.Service.Metrics.Services.Interfaces;
using HealtChecker.Shared.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Diagnostics;
using System.Text;

namespace HealtChecker.Service.Logging.Services.Implementations
{
    public class RabbitMqService : IRabbitMqService
    {
        private IServiceProvider _serviceProvider { get; init; }
        private string _logQueueName { get; init; }

        public RabbitMqService(
            IConfiguration configuration,
            IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            try
            {
                ConnectionFactory connectionFactory = new ConnectionFactory()
                {
                    HostName = configuration["RabbitMq.HostName"]
                };
                IConnection connection = connectionFactory.CreateConnection();

                IModel _logChannel = connection.CreateModel();

                _logQueueName = configuration["RabbitMq.LoggingQueue"];

                _logChannel.QueueDeclare(_logQueueName, false, false, false, null);

                EventingBasicConsumer consumer = new EventingBasicConsumer(_logChannel);
                consumer.Received += Consumer;

                _logChannel.BasicConsume(_logQueueName, true, consumer);
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);
                Trace.TraceError(ex.StackTrace);
            }
        }

        private void Consumer(object sender, BasicDeliverEventArgs args)
        {
            try
            {
                byte[] body = args.Body.ToArray();
                string message = Encoding.UTF8.GetString(body);

                LogItem logItem = JsonConvert.DeserializeObject<LogItem>(message);

                using (var scope = _serviceProvider.CreateScope())
                {
                    ILogService logService = scope.ServiceProvider.GetRequiredService<ILogService>();

                    logService.InsertLog(logItem);
                }

            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);
                Trace.TraceError(ex.StackTrace);
            }
        }
    }
}