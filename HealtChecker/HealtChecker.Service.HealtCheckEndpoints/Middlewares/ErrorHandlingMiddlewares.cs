using HealtChecker.Service.HealtCheckEndpoints.Services.Interfaces;
using HealtChecker.Shared.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace HealtChecker.Service.HealtCheckEndpoints.Middlewares
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
                LogItem logItem = LogItem.CreateLogItemFromException(ex);
                _rabbitMqService.PushLog(logItem);

                await httpContext.Response.WriteAsJsonAsync(new ServiceResult<bool>()
                {
                    Data = false,
                    ErrorMessage = logItem.ErrorMessage
                });
            }
        }
    }
}
