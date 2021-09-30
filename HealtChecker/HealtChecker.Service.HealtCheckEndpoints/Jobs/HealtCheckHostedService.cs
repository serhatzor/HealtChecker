using HealtChecker.Service.HealtCheckEndpoints.Models;
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
        private IHealtCheckCallService _healtCheckCallService { get; init; }
        private int _eachTickRecordCount { get; init; } = 20;
        private double _jobInterval { get; init; } = 10;

        public HealtCheckHostedService(
            IServiceProvider serviceProvider, 
            IConfiguration configuration, 
            IRabbitMqService rabbitMqService, 
            IHealtCheckCallService healtCheckCallService)
        {
            _serviceProvider = serviceProvider;
            _rabbitMqService = rabbitMqService;
            _healtCheckCallService = healtCheckCallService;
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
            List<HealtCheckEndpointModel> healtCheckEndpoints = null;
            using (var scope = _serviceProvider.CreateScope())
            {
                IHealtCheckEndpointService healtCheckEndpointService = scope.ServiceProvider.GetRequiredService<IHealtCheckEndpointService>();


                healtCheckEndpoints = healtCheckEndpointService
                    .GetExecutableHealtCheckEndpoints(_eachTickRecordCount)
                    .Result.Data;
            }

            if (healtCheckEndpoints == null)
                return;

            Task[] taskList = new Task[healtCheckEndpoints.Count];
            for (int i = 0; i < healtCheckEndpoints.Count; i++)
            {
                taskList[i] = HandleHealtCheck(healtCheckEndpoints[i]);
            }

            Task.WaitAll(taskList, int.MaxValue);
        }


        private async Task HandleHealtCheck(HealtCheckEndpointModel healtCheckEndpoint)
        {
            MetricItem metric = await _healtCheckCallService.GetMetric(healtCheckEndpoint);
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
