using HealtChecker.Service.Logging.Data.Entities;
using HealtChecker.Service.Logging.Data.Interfaces;
using HealtChecker.Service.Logging.Services.Interfaces;
using HealtChecker.Shared.Models;
using System;
using System.Threading.Tasks;

namespace HealtChecker.Service.Logging.Services.Implementations
{
    public class LogService : ILogService
    {
        private ILogDbContext _logDbContext { get; init; }
        public LogService(ILogDbContext logDbContext)
        {
            _logDbContext = logDbContext;
        }
        public async Task<ServiceResult<Guid>> InsertLog(LogItem logItem)
        {
            Log insertedModel = new Log()
            {
                Content = logItem.Content,
                ErrorTime = logItem.ErrorTime,
                Id = logItem.Id,
                LogType = logItem.LogType,
                ErrorMessage = logItem.ErrorMessage,
                Channel = logItem.Channel
            };

            await _logDbContext.Logs.AddAsync(insertedModel);

            await _logDbContext.SaveChangesAsync();

            ServiceResult<Guid> result = new ServiceResult<Guid>();
            result.Data = logItem.Id;

            return result;
        }
    }
}