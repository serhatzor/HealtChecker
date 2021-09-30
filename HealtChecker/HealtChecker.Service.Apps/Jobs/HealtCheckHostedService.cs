using HealtChecker.Service.Metrics.Models;
using HealtChecker.Service.Metrics.Services.Interfaces;
using HealtChecker.Service.HealtCheckEndpoints.Services.Interfaces;
using HealtChecker.Shared.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace HealtChecker.Service.HealtCheckEndpoints.Services.Jobs
{
    public class HealtCheckHostedService : IHostedService, IDisposable
    {
        private Timer _timer { get; set; }
        private IServiceProvider _serviceProvider { get; init; }
        private IRabbitMqService _rabbitMqService { get; init; }
        private int _eachTickRecordCount { get; init; } = 20;
        private double _jobInterval { get; init; } = 10;

        public HealtCheckHostedService(IServiceProvider serviceProvider, IConfiguration configuration, IRabbitMqService rabbitMqService)
        {
            _serviceProvider = serviceProvider;
            _rabbitMqService = rabbitMqService;
            if (Int32.TryParse(configuration["Job.EachTickRecordCount"], out int eachTickRecordCount))
            {
                _eachTickRecordCount = eachTickRecordCount;
            }
            if (Double.TryParse(configuration["Job.EachTickRecordCount"], out double jobInterval))
            {
                _jobInterval = jobInterval;
            }
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(_jobInterval));
            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            IHealtCheckEndpointService healtCheckEndpointService = _serviceProvider.GetRequiredService<IHealtCheckEndpointService>();


            List<HealtCheckEndpointModel> healtCheckEndpoints = healtCheckEndpointService
                .GetExecutableHealtCheckEndpoints(_eachTickRecordCount)
                .Result.Data;

            Task[] taskList = new Task[healtCheckEndpoints.Count];
            for (int i = 0; i < healtCheckEndpoints.Count; i++)
            {
                taskList[i] = HandleHealtCheck(healtCheckEndpoints[i]);
            }

            Task.WaitAll(taskList, int.MaxValue);
        }


        private async Task HandleHealtCheck(HealtCheckEndpointModel healtCheckEndpoint)
        {
            Metric metric = new Metric();
            using (HttpClient client = new HttpClient())
            {
                DateTime startTime = DateTime.UtcNow;

                HttpResponseMessage getHttpResponse = null;
                try
                {
                    getHttpResponse = await client.GetAsync(healtCheckEndpoint.HealtCheckUrl);
                    getHttpResponse.EnsureSuccessStatusCode();
                    metric.Description = getHttpResponse.ReasonPhrase;
                }
                catch (Exception ex)
                {
                    metric.Description = ex.Message;
                }

                TimeSpan timeSpan = DateTime.UtcNow - startTime;

                metric.ExecutionSeconds = timeSpan.TotalSeconds;
                metric.HealtCheckEndpointId = healtCheckEndpoint.Id;
                metric.HttpStatusCode = getHttpResponse == null ? HttpStatusCode.NoContent : getHttpResponse.StatusCode;
                metric.ConnectedUserId = healtCheckEndpoint.ConnectedUserId;
                metric.HealtCheckUrl = healtCheckEndpoint.HealtCheckUrl;

            }

            _rabbitMqService.PushMetric(metric);
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
