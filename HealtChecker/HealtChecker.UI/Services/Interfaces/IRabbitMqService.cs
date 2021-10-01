using HealtChecker.Shared.Models;

namespace HealtChecker.UI.Services.Interfaces
{
    public interface IRabbitMqService
    {
        void PushLog(LogItem log);

    }
}
