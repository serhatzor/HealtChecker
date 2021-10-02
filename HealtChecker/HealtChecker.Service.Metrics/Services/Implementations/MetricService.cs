using HealtChecker.Service.Metrics.Data.Entities;
using HealtChecker.Service.Metrics.Data.Interfaces;
using HealtChecker.Service.Metrics.Services.Interfaces;
using HealtChecker.Shared.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<ServiceResult<List<MetricItem>>> GetMetricsByHealtCheckEndpointId(Guid endpointId, Guid userId)
        {
            List<Metric> metrics = await _metricsDbContext.Metrics.Where(
                x => x.HealtCheckEndpointId == endpointId && x.ConnectedUserId == userId).ToListAsync();

            ServiceResult<List<MetricItem>> result =
                new ServiceResult<List<MetricItem>>();

            result.Data = EntityToModel(metrics);


            return result;
        }

        public async Task<ServiceResult<MetricItem>> GetLastSuccessMetric(Guid endpointId)
        {
            Metric metric = await _metricsDbContext.Metrics.Where(
                x => x.HealtCheckEndpointId == endpointId &&
                (int)x.HttpStatusCode >= 200 && (int)x.HttpStatusCode <= 299)
                .OrderByDescending(x => x.CreatedAt).FirstOrDefaultAsync();

            ServiceResult<MetricItem> result = new ServiceResult<MetricItem>();
            if (metric != null)
                result.Data = EntityToModel(new List<Metric>() { metric })[0];

            return result;
        }



        private List<MetricItem> EntityToModel(List<Metric> metrics)
        {
            return metrics.Select(x => new MetricItem()
            {
                ConnectedUserId = x.ConnectedUserId,
                Content = x.Content,
                Description = x.Description,
                ExecutionSeconds = x.ExecutionSeconds,
                HealtCheckEndpointId = x.HealtCheckEndpointId,
                HealtCheckUrl = x.HealtCheckUrl,
                HttpStatusCode = x.HttpStatusCode,
                Id = x.Id,
                CreatedAt = x.CreatedAt
            }).ToList();
        }


        public async Task<ServiceResult<Guid>> InsertMetric(MetricItem metricItem)
        {
            Metric insertedMetric = new Metric()
            {
                ConnectedUserId = metricItem.ConnectedUserId,
                Content = metricItem.Content,
                Description = metricItem.Description,
                ExecutionSeconds = metricItem.ExecutionSeconds,
                HealtCheckEndpointId = metricItem.HealtCheckEndpointId,
                HealtCheckUrl = metricItem.HealtCheckUrl,
                HttpStatusCode = metricItem.HttpStatusCode
            };
            await _metricsDbContext.Metrics.AddAsync(insertedMetric);

            await _metricsDbContext.SaveChangesAsync();
            ServiceResult<Guid> result = new ServiceResult<Guid>();

            result.Data = insertedMetric.Id;

            return result;
        }
    }
}
