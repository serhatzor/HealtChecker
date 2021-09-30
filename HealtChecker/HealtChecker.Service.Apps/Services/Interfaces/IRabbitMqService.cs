using HealtChecker.Shared.Models;

namespace HealtChecker.Service.HealtCheckEndpoints.Services.Interfaces
{
    public interface IRabbitMqService
    {
        void PushLog(LogItem log);
        void PushMetric(Metric metric);
    }
}
