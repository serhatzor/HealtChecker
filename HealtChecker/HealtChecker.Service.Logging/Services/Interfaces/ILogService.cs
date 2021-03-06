using HealtChecker.Shared.Models;
using System;
using System.Threading.Tasks;

namespace HealtChecker.Service.Logging.Services.Interfaces
{
    public interface ILogService
    {
        Task<ServiceResult<Guid>> InsertLog(LogItem logItem);
    }
}