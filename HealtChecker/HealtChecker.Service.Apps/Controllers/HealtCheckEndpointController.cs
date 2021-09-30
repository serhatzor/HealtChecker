using HealtChecker.Service.Metrics.Base;
using HealtChecker.Service.Metrics.Models;
using HealtChecker.Service.Metrics.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HealtChecker.Service.Metrics.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealtCheckEndpointController : ControllerBase
    {
        private IHealtCheckEndpointService _healtCheckEndpointService { get; init; }
        public HealtCheckEndpointController(IHealtCheckEndpointService healtCheckEndpointService)
        {
            _healtCheckEndpointService = healtCheckEndpointService;
        }

        [HttpGet("GetById/{healtCheckEndPointId}")]
        public async Task<ServiceResult<HealtCheckEndpointModel>> GetById(Guid healtCheckEndPointId)
        {
            return await _healtCheckEndpointService.GetHealtCheckEnpointById(new HealtCheckEndpointModel()
            {
                Id = healtCheckEndPointId
            });
        }

        [HttpGet("GetByUserId/{connectedUserId}")]
        public async Task<ServiceResult<List<HealtCheckEndpointModel>>> GetByUserId(Guid connectedUserId)
        {
            return await _healtCheckEndpointService.GetHealtCheckEnpointsByUserId(new HealtCheckEndpointModel
            {
                ConnectedUserId = connectedUserId
            });
        }

        [HttpPost]
        public async Task<ServiceResult<Guid>> Post([FromBody] HealtCheckEndpointModel healtCheckEndpointModel)
        {
            return await _healtCheckEndpointService.CreateHealtCheckEndpoint(healtCheckEndpointModel);
        }

        [HttpPut]
        public async Task<ServiceResult<bool>> Put([FromBody] HealtCheckEndpointModel healtCheckEndpointModel)
        {
            return await _healtCheckEndpointService.UpdateHealtCheckEndpoint(healtCheckEndpointModel);
        }

        [HttpDelete("{healtCheckEndPointId}")]
        public async Task<ServiceResult<bool>> Delete(Guid healtCheckEndPointId)
        {
            return await _healtCheckEndpointService.DeleteHealtCheckEndpoint(new HealtCheckEndpointModel()
            {
                Id = healtCheckEndPointId
            });
        }
    }
}
