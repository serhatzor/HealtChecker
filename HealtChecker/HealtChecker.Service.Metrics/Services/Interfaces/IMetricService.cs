using HealtChecker.Shared.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HealtChecker.Service.Metrics.Services.Interfaces
{
    public interface IMetricService
    {
        Task<ServiceResult<Guid>> InsertMetric(MetricItem metric);
        Task<ServiceResult<List<MetricItem>>> GetMetricsByHealtCheckEndpointId(Guid endpointId, Guid connectedUserId);
        Task<ServiceResult<MetricItem>> GetLastSuccessMetric(Guid endpointId);
    }
}
