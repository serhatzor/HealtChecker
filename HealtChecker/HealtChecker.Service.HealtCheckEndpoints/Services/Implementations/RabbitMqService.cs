using HealtChecker.Service.HealtCheckEndpoints.Services.Interfaces;
using HealtChecker.Shared.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Diagnostics;
using System.Text;

namespace HealtChecker.Service.HealtCheckEndpoints.Services.Implementations
{
    public class RabbitMqService : IRabbitMqService
    {
        private IModel _logChannel { get; init; }
        private IModel _metricChannel { get; init; }
        private string _logQueueName { get; init; }
        private string _metricQueueName { get; init; }

        public RabbitMqService(IConfiguration configuration)
        {
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

        public void PushMetric(MetricItem metric)
        {
            if (_metricChannel == null)
            {
                Trace.TraceError(JsonConvert.SerializeObject(metric));
            }
            try
            {
                _metricChannel?.BasicPublish(string.Empty, _metricQueueName, null, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(metric)));
            }
            catch (Exception ex)
            {
                Trace.TraceError(JsonConvert.SerializeObject(metric));
                Trace.TraceError(ex.Message);
                Trace.TraceError(ex.StackTrace);
            }
        }
    }
}
