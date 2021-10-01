using HealtChecker.Shared.Models;
using HealtChecker.UI.Services.Implementations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HealtChecker.UI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class HealtCheckApiController : ControllerBase
    {
        private HealtCheckService _healtCheckService { get; set; }
        private MetricService _metricService { get; set; }
        public HealtCheckApiController(HealtCheckService healtCheckService, MetricService metricService)
        {
            _healtCheckService = healtCheckService;
            _metricService = metricService;
        }
        [HttpPost]
        public async Task<ServiceResult<Guid>> Post([FromBody] HealtCheckEndpointModel healtCheckEndpointModel)
        {
            return await _healtCheckService.InsertHealtCheckEndPoint(healtCheckEndpointModel);
        }

        [HttpPut]
        public async Task<ServiceResult<bool>> Update([FromBody] HealtCheckEndpointModel healtCheckEndpointModel)
        {
            return await _healtCheckService.UpdateHealtCheckEndPoint(healtCheckEndpointModel);
        }

        [HttpGet("{id}")]
        public async Task<ServiceResult<HealtCheckEndpointModel>> GetHealtCheckById(Guid id)
        {
            return await _healtCheckService.GetHealtCheckById(id);
        }

        [HttpGet("GetMetricsHealtCheckById/{id}")]
        public async Task<ServiceResult<List<MetricItem>>> GetMetricsHealtCheckById(Guid id)
        {
            return await _metricService.GetByHealtCheckEndPointIdOperation(id);
        }

        [HttpDelete("{id}")]
        public async Task<ServiceResult<bool>> Delete(Guid id)
        {
            return await _healtCheckService.Delete(id);
        }


    }
}
