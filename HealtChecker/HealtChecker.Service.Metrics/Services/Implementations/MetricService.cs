using HealtChecker.Service.Metrics.Data.Entities;
using HealtChecker.Service.Metrics.Data.Interfaces;
using HealtChecker.Service.Metrics.Services.Interfaces;
using HealtChecker.Shared.Models;
using System;
using System.Threading.Tasks;

namespace HealtChecker.Service.Metrics.Services.Implementations
{
    public class MetricService : IMetricService
    {
        private IMetricsDbContext _metricsDbContext { get; init; }
        public MetricService(IMetricsDbContext metricsDbContext)
        {
            _metricsDbContext = metricsDbContext;
        }
        public async Task<ServiceResult<bool>> InsertMetric(MetricItem metric)
        {
            await _metricsDbContext.Metrics.AddAsync(new Metric()
            {
                ConnectedUserId = metric.ConnectedUserId,
                Content = metric.Content,
                Description = metric.Description,
                ExecutionSeconds = metric.ExecutionSeconds,
                HealtCheckEndpointId = metric.HealtCheckEndpointId,
                HealtCheckUrl = metric.HealtCheckUrl,
                HttpStatusCode = metric.HttpStatusCode
            });

            ServiceResult<bool> result = new ServiceResult<bool>();

            result.Data = await _metricsDbContext.SaveChangesAsync() > 0;

            return result;
        }
    }
}
