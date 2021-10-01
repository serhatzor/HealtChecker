using HealtChecker.Service.Logging.Data.Implementations;
using HealtChecker.Service.Logging.Data.Interfaces;
using HealtChecker.Service.Logging.Services.Implementations;
using HealtChecker.Service.Logging.Services.Interfaces;
using HealtChecker.Service.Metrics.Services.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace HealtChecker.Service.Logging
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
            services.AddSingleton<IRabbitMqService, RabbitMqService>();
            services.AddTransient<ILogService, LogService>();
            services.AddDbContext<ILogDbContext, LogDbContext>(
                (contextOptions) =>
                {
                    contextOptions.UseInMemoryDatabase("LogDbContext");
                }
            );

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            var rabbitMqService = serviceProvider.GetRequiredService<IRabbitMqService>();
        }
    }
}