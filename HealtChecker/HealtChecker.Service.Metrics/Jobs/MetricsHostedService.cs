using HealtChecker.Service.Metrics.Services.Interfaces;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace HealtChecker.Service.Metrics.Jobs
{
    public class MetricsHostedService : IHostedService
    {
        private IRabbitMqService _rabbitMqService { get; set; }
        public MetricsHostedService(IRabbitMqService rabbitMqService)
        {
            _rabbitMqService = rabbitMqService;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            _rabbitMqService.Init();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            return Task.CompletedTask;
        }

    }
}
