using HealtChecker.Shared.Models;
using HealtChecker.UI.Data;
using HealtChecker.UI.Services.Implementations;
using HealtChecker.UI.Services.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace HealtChecker.UI
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
            services.AddDbContext<ApplicationDbContext>(
                (contextOptions) =>
                {
                    contextOptions.UseInMemoryDatabase("ApplicationDbContext");
                }
            );
            services.AddSingleton<IRabbitMqService, RabbitMqService>();
            services.AddHttpClient<HealtCheckService>((c) =>
            {
                c.BaseAddress = new Uri(Configuration["HealtCheckEndpoints.Url"]);
            });
            services.AddHttpClient<MetricService>((c) =>
            {
                c.BaseAddress = new Uri(Configuration["Metrics.Url"]);
            });
            services.AddDatabaseDeveloperPageExceptionFilter();
            services.AddDefaultIdentity<IdentityUser>((options) =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.SignIn.RequireConfirmedEmail = false;
                options.SignIn.RequireConfirmedPhoneNumber = false;
            })
                .AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddRazorPages();
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.ContentType = "application/json";
                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                    if (contextFeature != null)
                    {
                        LogItem logItem = LogItem.CreateLogItemFromException(contextFeature.Error, Channel.UI);

                        IRabbitMqService rabbitMqService = serviceProvider.GetRequiredService<IRabbitMqService>();
                        rabbitMqService.PushLog(logItem);

                        if(context.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                        {
                            await context.Response.WriteAsJsonAsync(new ServiceResult<bool>()
                            {
                                Data = false,
                                ErrorMessage = logItem.ErrorMessage
                            });

                        }
                    }
                });
            });

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
            });
        }
    }
}
