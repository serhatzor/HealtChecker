using HealtChecker.Service.HealtCheckEndpoints.Data.Implementations;
using HealtChecker.Service.HealtCheckEndpoints.Data.Interfaces;
using HealtChecker.Service.HealtCheckEndpoints.Middlewares;
using HealtChecker.Service.HealtCheckEndpoints.Services.Implementations;
using HealtChecker.Service.HealtCheckEndpoints.Services.Interfaces;
using HealtChecker.Service.HealtCheckEndpoints.Services.Jobs;
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

namespace HealtChecker.Service.HealtCheckEndpoints
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
            services.AddDbContext<IHealtCheckDbContext, HealtCheckDbContext>(
                (contextOptions) =>
                {
                    contextOptions.UseInMemoryDatabase("HealtCheckDbContext");
                }
            );

            services.AddSingleton<IHealtCheckCallService, HealtCheckCallService>();
            services.AddSingleton<IRabbitMqService, RabbitMqService>();
            services.AddHostedService<HealtCheckHostedService>();

            services.AddTransient<IHealtCheckEndpointService, HealtCheckEndpointService>();
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
                        LogItem logItem = LogItem.CreateLogItemFromException(contextFeature.Error);

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
