using HealtChecker.Service.Metrics.Services.Interfaces;
using HealtChecker.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HealtChecker.Service.HealtCheckEndpoints.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MetricsController : ControllerBase
    {
        private IMetricService _metricService { get; init; }
        public MetricsController(IMetricService metricService)
        {
            _metricService = metricService;
        }

        [HttpGet("GetByHealtCheckEndpointId/{endpointId}/{userId}")]
        public async Task<ServiceResult<List<MetricItem>>> GetByHealtCheckEndpointId(Guid endpointId,Guid userId)
        {
            return await _metricService.GetMetricsByHealtCheckEndpointId(endpointId, userId);
        }

        [HttpPost]
        public async Task<ServiceResult<Guid>> Post([FromBody] MetricItem metricItem)
        {
            return await _metricService.InsertMetric(metricItem);
        }
    }
}
