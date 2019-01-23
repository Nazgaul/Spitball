using System;
using System.Globalization;
using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Cloudents.Core;
using Cloudents.Core.Extension;
using Cloudents.Core.Interfaces;
using Cloudents.Infrastructure.Storage;
using Cloudents.Web.Middleware;
using Joonasw.AspNetCore.SecurityHeaders;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Rewrite;
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
            services.AddMvc()
                .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
                .AddMvcOptions(o =>
                {
                    o.Filters.Add(new ResponseCacheAttribute
                    {
                        NoStore = true,
                        Location = ResponseCacheLocation.None
                    });
                    //o.Filters.Add(new o.Filters.Add(new ResponseCacheFilter(new CacheProfile())))
                });
            services.AddHsts(options =>
            {
                options.MaxAge = TimeSpan.FromDays(365);
                options.IncludeSubDomains = true;
                options.Preload = true;
            });
            var containerBuilder = new ContainerBuilder();
            var assembliesOfProgram = new[]
            {
                Assembly.Load("Cloudents.Infrastructure.Storage"),
                Assembly.Load("Cloudents.Core"),
                Assembly.GetExecutingAssembly()
            };
            var keys = new ConfigurationKeys("https://www.spitball.co")
            {
                
                Storage = Configuration["Storage"]
            };

            containerBuilder.Register(_ => keys).As<IConfigurationKeys>();
            containerBuilder.RegisterModule<ModuleStorage>();
            //containerBuilder.RegisterSystemModules(
                //Core.Enum.System.IcoSite, assembliesOfProgram);

            //containerBuilder.RegisterType<Logger>().As<ILogger>();
            containerBuilder.Populate(services);
            var container = containerBuilder.Build();
            return new AutofacServiceProvider(container);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseClickJacking();
            app.UseCsp(csp =>
            {
                csp.AllowImages.FromSelf().OnlyOverHttps();
                csp.AllowFonts.FromSelf().OnlyOverHttps();
                csp.AllowStyles.FromSelf().AllowUnsafeInline().OnlyOverHttps();
                csp.AllowScripts.FromSelf().AllowUnsafeInline().OnlyOverHttps();
                csp.AllowFrames.From("https://www.youtube.com/").OnlyOverHttps();
                csp.AllowConnections.OnlyOverHttps().ToSelf();
                csp.ByDefaultAllow.FromNowhere();
            });
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                var reWriterOptions = new RewriteOptions();
                reWriterOptions.AddRedirectToHttpsPermanent();

                app.UseRewriter(reWriterOptions);
                app.UseExceptionHandler("/Home/Error");
                HstsBuilderExtensions.UseHsts(app);

            }

            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture(SupportedCultures[0]),

                // Formatting numbers, dates, etc.
                SupportedCultures = SupportedCultures,
                // UI strings that we have localized.
                SupportedUICultures = SupportedCultures
            });
            app.UseStaticFiles(new StaticFileOptions
            {
                OnPrepareResponse = ctx =>
                {
                    ctx.Context.Response.Headers.Add("Cache-Control", "public,max-age=864000");
                }
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
