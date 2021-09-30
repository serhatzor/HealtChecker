using HealtChecker.Shared.Models;

namespace HealtChecker.Service.Metrics.Services.Interfaces
{
    public interface IRabbitMqService
    {
        void PushLog(LogItem log);
    }
}
