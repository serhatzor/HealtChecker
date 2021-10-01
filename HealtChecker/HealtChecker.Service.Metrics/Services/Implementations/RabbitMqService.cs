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

namespace HealtChecker.Service.Metrics.Services.Implementations
{
    public class RabbitMqService : IRabbitMqService
    {
        private IModel _logChannel { get; set; }
        private IModel _metricChannel { get; set; }
        private IServiceProvider _serviceProvider { get; set; }
        private IConfiguration _configuration { get; set; }
        private string _logQueueName { get; set; }
        private string _metricQueueName { get; set; }

        public RabbitMqService(
            IConfiguration configuration,
            IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _configuration = configuration;
        }

        public void Init()
        {
            try
            {
                ConnectionFactory connectionFactory = new ConnectionFactory()
                {
                    HostName = _configuration["RabbitMq.HostName"]
                };
                IConnection connection = connectionFactory.CreateConnection();

                _logChannel = connection.CreateModel();
                _metricChannel = connection.CreateModel();

                _logQueueName = _configuration["RabbitMq.LoggingQueue"];
                _metricQueueName = _configuration["RabbitMq.MetricsQueue"];

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
                PushLog(new LogItem(Channel.ServiceMetrics)
                {
                    Content = JsonConvert.SerializeObject(ex),
                    ErrorTime = DateTime.UtcNow,
                    Id = Guid.NewGuid(),
                    LogType = ex.GetType().FullName,
                });
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
