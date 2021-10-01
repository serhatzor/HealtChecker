using HealtChecker.Shared.Models;
using System.Threading.Tasks;

namespace HealtChecker.Service.Metrics.Services.Interfaces
{
    public interface IEmailNotificationService
    {
        void SendEmail(MetricItem metricItem);
    }
}
