using HealtChecker.Service.Metrics.Services.Interfaces;
using HealtChecker.Shared.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealtChecker.Service.Metrics.Services.Implementations
{
    public class RabbitMqService : IRabbitMqService
    {
        private IModel _logChannel { get; init; }
        private IModel _metricChannel { get; init; }
        private IServiceProvider _serviceProvider { get; init; }
        private string _logQueueName { get; init; }
        private string _metricQueueName { get; init; }

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
                IModel _metricChannel = connection.CreateModel();

                _logQueueName = configuration["RabbitMq.LoggingQueue"];
                _metricQueueName = configuration["RabbitMq.MetricsQueue"];

                _logChannel.QueueDeclare(_logQueueName, false, false, false, null);
                _metricChannel.QueueDeclare(_metricQueueName, false, false, false, null);

                EventingBasicConsumer consumer = new EventingBasicConsumer(_metricChannel);
                consumer.Received += Consumer;

                _metricChannel.BasicConsume(_metricQueueName, true, consumer);

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

                MetricItem metric = JsonConvert.DeserializeObject<MetricItem>(message);

                using (var scope = _serviceProvider.CreateScope())
                {
                    IMetricService metricService = scope.ServiceProvider.GetRequiredService<IMetricService>();

                    metricService.InsertMetric(metric);
                }

            }
            catch (Exception ex)
            {
                PushLog(new LogItem()
                {
                    Content = JsonConvert.SerializeObject(ex),
                    ErrorTime = DateTime.UtcNow,
                    Id = Guid.NewGuid(),
                    LogType = ex.GetType().FullName
                });
            }
        }

        public void PushLog(LogItem log)
        {
            if (_logChannel == null)
            {
                Trace.TraceError(JsonConvert.SerializeObject(log));
            }
            try
            {
                _logChannel?.BasicPublish(string.Empty, _logQueueName, null, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(log)));
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
