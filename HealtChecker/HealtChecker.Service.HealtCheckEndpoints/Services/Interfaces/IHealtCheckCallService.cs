using HealtChecker.Service.HealtCheckEndpoints.Models;
using HealtChecker.Shared.Models;
using System.Threading.Tasks;

namespace HealtChecker.Service.HealtCheckEndpoints.Services.Interfaces
{
    public interface IHealtCheckCallService
    {
        Task<MetricItem> GetMetric(HealtCheckEndpointModel healtCheckEndpoint);
    }
}
