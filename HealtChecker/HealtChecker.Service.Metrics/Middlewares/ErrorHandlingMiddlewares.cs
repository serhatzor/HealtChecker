using HealtChecker.Service.Metrics.Services.Interfaces;
using HealtChecker.Shared.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace HealtChecker.Service.Metrics.Middlewares
{
    public class ErrorHandlingMiddleware
    {
        private RequestDelegate _next { get; init; }
        private IRabbitMqService _rabbitMqService { get; init; }

        public ErrorHandlingMiddleware(RequestDelegate next, IRabbitMqService rabbitMqService)
        {
            _next = next;
            _rabbitMqService = rabbitMqService;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                Guid logId = HandleException(ex);
                await httpContext.Response.WriteAsJsonAsync(new ServiceResult<bool>()
                {
                    Data = false,
                    Exception = new Exception($"Unexpected error occured with ReferenceId : {logId}")
                });
            }
        }

        private Guid HandleException(Exception ex)
        {
            Guid logId = Guid.NewGuid();
            LogItem logItem = new LogItem()
            {
                Content = JsonConvert.SerializeObject(ex),
                ErrorTime = DateTime.UtcNow,
                LogType = ex.GetType().FullName,
                Id = logId
            };

            _rabbitMqService.PushLog(logItem);

            return logId;

        }
    }
}
