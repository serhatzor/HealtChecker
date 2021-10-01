using HealtChecker.Service.Metrics.Data.Implementations;
using HealtChecker.Service.Metrics.Data.Interfaces;
using HealtChecker.Service.Metrics.Jobs;
using HealtChecker.Service.Metrics.Middlewares;
using HealtChecker.Service.Metrics.Services.Implementations;
using HealtChecker.Service.Metrics.Services.Interfaces;
using HealtChecker.Shared.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Net;

namespace HealtChecker.Service.Metrics
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<IMetricsDbContext, MetricsDbContext>(
                (contextOptions) =>
                {
                    contextOptions.UseInMemoryDatabase("MetricsDbContext");
                }
            );
            services.AddScoped<IMetricService, MetricService>();
            services.AddScoped<IDetectDownTimeService, DetectDownTimeService>();
            services.AddSingleton<IRabbitMqService, RabbitMqService>();
            services.AddSingleton<IEmailNotificationService, EmailNotificationService>();
            services.AddHostedService<MetricsHostedService>();
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            app.UseMiddleware(typeof(ErrorHandlingMiddleware));

            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.ContentType = "application/json";
                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                    if (contextFeature != null)
                    {
                        LogItem logItem = LogItem.CreateLogItemFromException(contextFeature.Error, Channel.ServiceMetrics);

                        IRabbitMqService rabbitMqService = serviceProvider.GetRequiredService<IRabbitMqService>();
                        rabbitMqService.PushLog(logItem);

                        await context.Response.WriteAsJsonAsync(new ServiceResult<bool>()
                        {
                            Data = false,
                            ErrorMessage = logItem.ErrorMessage
                        });
                    }
                });
            });


            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
