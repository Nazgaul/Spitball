using System;
using System.Globalization;
using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Cloudents.Core;
using Cloudents.Core.Extension;
using Cloudents.Core.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Cloudents.Ico
{
    public class Startup
    {
        public static readonly CultureInfo[] SupportedCultures = new[]
         {

            new CultureInfo("en-US"),
            new CultureInfo("es-ES"),
            new CultureInfo("de"),
            new CultureInfo("ru"),
            new CultureInfo("zh-Hans"),
            new CultureInfo("ko"),
            new CultureInfo("ja"),
        };

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddLocalization(x => x.ResourcesPath = "Resources");
            services.AddMvc().AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix);
            services.AddHsts(options =>
            {
                options.MaxAge = TimeSpan.FromDays(365);
                options.IncludeSubDomains = true;
                options.Preload = true;
            });
            var containerBuilder = new ContainerBuilder();
            var assembliesOfProgram = new[]
            {
                Assembly.Load("Cloudents.Infrastructure.Framework"),
                Assembly.Load("Cloudents.Core"),
                Assembly.GetExecutingAssembly()
            };
            var keys = new ConfigurationKeys("https://www.spitball.co")
            {
                ServiceBus = Configuration["ServiceBus"]
            };

            containerBuilder.Register(_ => keys).As<IConfigurationKeys>();
            containerBuilder.RegisterSystemModules(
                Core.Enum.System.IcoSite, assembliesOfProgram);

            //containerBuilder.RegisterType<Logger>().As<ILogger>();
            containerBuilder.Populate(services);
            var container = containerBuilder.Build();
            return new AutofacServiceProvider(container);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
                
            }

            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture(SupportedCultures[0]),

                // Formatting numbers, dates, etc.
                SupportedCultures = SupportedCultures,
                // UI strings that we have localized.
                SupportedUICultures = SupportedCultures
            });
            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
