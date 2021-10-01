using HealtChecker.Service.Metrics.Services.Interfaces;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace HealtChecker.Service.Logging.Jobs
{
    public class LoggingHostedService : IHostedService
    {
        private IRabbitMqService _rabbitMqService { get; set; }
        public LoggingHostedService(IRabbitMqService rabbitMqService)
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
