using HealtChecker.Shared.Models;

namespace HealtChecker.Service.Metrics.Services.Interfaces
{
    public interface IEmailNotificationService
    {
        void SendEmail(MetricItem metricItem);
    }
}
