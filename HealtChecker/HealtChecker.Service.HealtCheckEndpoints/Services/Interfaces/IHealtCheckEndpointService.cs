using HealtChecker.Shared.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HealtChecker.Service.HealtCheckEndpoints.Services.Interfaces
{
    public interface IHealtCheckEndpointService
    {
        Task<ServiceResult<Guid>> CreateHealtCheckEndpoint(HealtCheckEndpointModel healtCheckEndpointModel);
        Task<ServiceResult<bool>> UpdateHealtCheckEndpoint(HealtCheckEndpointModel HealtCheckEndpointModel);
        Task<ServiceResult<bool>> DeleteHealtCheckEndpoint(HealtCheckEndpointModel HealtCheckEndpointModel);
        Task<ServiceResult<HealtCheckEndpointModel>> GetHealtCheckEnpointById(HealtCheckEndpointModel HealtCheckEndpointModel);
        Task<ServiceResult<List<HealtCheckEndpointModel>>> GetHealtCheckEnpointsByUserId(HealtCheckEndpointModel HealtCheckEndpointModel);
        Task<ServiceResult<List<HealtCheckEndpointModel>>> GetExecutableHealtCheckEndpoints(int recordCount);
    }
}
