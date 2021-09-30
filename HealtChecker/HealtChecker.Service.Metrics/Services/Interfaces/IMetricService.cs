using HealtChecker.Shared.Models;
using System.Threading.Tasks;

namespace HealtChecker.Service.Metrics.Services.Interfaces
{
    public interface IMetricService
    {
        Task<ServiceResult<bool>> InsertMetric(MetricItem metric);
    }
}
