using System.Collections.Generic;
using Autofac;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using VideoAnalytics.Web.Configuration;
using VideoAnalytics.Web.Configuration.Interfaces;
using VideoAnalytics.Web.Models;
using VideoAnalytics.Web.Services;
using VideoAnalytics.Web.Services.Interfaces;

namespace VideoAnalytics.Web
{
    public class Startup
    {
        public Startup(IWebHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            this.Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        public ILifetimeScope AutofacContainer { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews().AddNewtonsoftJson();

            // In production, the React files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/build";
            });
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            var projectSettings = new CustomVisionProjectSettings
            {
                ProjectName = Configuration.GetValue<string>("CustomVision:Project:Name"),
                ProjectType = Configuration.GetValue<string>("CustomVision:Project:Type")
            };

            var authoringSettings = new CustomVisionAuthoringSettings
            {
                AccountRegion = Configuration.GetValue<string>("CustomVision:AuthoringService:AccountRegion"),
                AccountName = Configuration.GetValue<string>("CustomVision:AuthoringService:AccountName"),
                AccountKey = Configuration.GetValue<string>("CustomVision:AuthoringService:AccountKey")
            };

            var predictionSettings = new CustomVisionPredictionSettings
            {
                AccountRegion = Configuration.GetValue<string>("CustomVision:PredictionService:AccountRegion"),
                AccountName = Configuration.GetValue<string>("CustomVision:PredictionService:AccountName"),
                AccountKey = Configuration.GetValue<string>("CustomVision:PredictionService:AccountKey")
            };

            builder.Register(ctx =>
            {
                var systemSettings = new SystemSettings
                {
                    WorkingDirectory = Configuration.GetValue<string>("FileSystem:WorkingDirectory")
                };
                return systemSettings;
            }).As<ISystemSettings>();

            builder.Register(ctx =>
            {
                var demoSettingsEntries = Configuration.GetSection("Demos").Get<List<DemoSetting>>();
                var demoSettings = new DemoSettings
                {
                    Settings = demoSettingsEntries
                };
                return demoSettings;
            }).As<IDemoSettings>();

            var customVisionProjectService = new CustomVisionProjectService(projectSettings, authoringSettings);
            builder.Register(ctx => customVisionProjectService).As<ICustomVisionProjectService>();

            builder.Register(ctx => new CustomVisionAuthoringService(customVisionProjectService, projectSettings, authoringSettings)).As<ICustomVisionAuthoringService>();

            builder.Register(ctx => new CustomVisionPredictionService(customVisionProjectService, projectSettings, predictionSettings)).As<ICustomVisionPredictionService>();

            builder.RegisterType<VideoFrameExtractionService>().As<IVideoFrameExtractionService>();

            // TODO: move to module as below
            // builder.RegisterModule(new MyApplicationModule());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseReactDevelopmentServer(npmScript: "start");
                }
            });
        }
    }
}
