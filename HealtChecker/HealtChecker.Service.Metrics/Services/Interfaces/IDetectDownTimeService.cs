using HealtChecker.Shared.Models;
using System.Threading.Tasks;

namespace HealtChecker.Service.Metrics.Services.Interfaces
{
    public interface IDetectDownTimeService
    {
        Task DetectDownTime(MetricItem metric);
    }
}
