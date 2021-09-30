using HealtChecker.Service.Metrics.Data.Implementations;
using HealtChecker.Service.Metrics.Data.Interfaces;
using HealtChecker.Service.Metrics.Middlewares;
using HealtChecker.Service.Metrics.Services.Implementations;
using HealtChecker.Service.Metrics.Services.Interfaces;
using HealtChecker.Service.HealtCheckEndpoints.Services.Implementations;
using HealtChecker.Service.HealtCheckEndpoints.Services.Interfaces;
using HealtChecker.Service.HealtCheckEndpoints.Services.Jobs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
            services.AddDbContext<IHealtCheckDbContext, HealtCheckDbContext>(
                (contextOptions) =>
                {
                    contextOptions.UseInMemoryDatabase("HealtCheckDbContext");
                }
            );

            services.AddSingleton<IRabbitMqService, RabbitMqService>();
            services.AddHostedService<HealtCheckHostedService>();

            services.AddTransient<IHealtCheckEndpointService, HealtCheckEndpointService>();
            services.AddControllers();
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMiddleware(typeof(ErrorHandlingMiddleware));

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
        }
    }
}
