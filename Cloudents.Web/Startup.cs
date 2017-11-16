using System;
using System.Globalization;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Cloudents.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Converters;

namespace Cloudents.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {

            services.AddLocalization(options => options.ResourcesPath = "Resources");
            services.AddMvc()
                .AddJsonOptions(options =>
            {
                options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
                options.SerializerSettings.Converters.Add(new StringEnumConverter { CamelCaseText = true });
                options.SerializerSettings.Converters.Add(new IsoDateTimeConverter
                {
                    DateTimeStyles = System.Globalization.DateTimeStyles.AssumeUniversal
                });
            });
            services.Configure<MvcOptions>(options =>
            {
                options.Filters.Add(new RequireHttpsAttribute());
            });
            services.AddResponseCompression();
            services.AddResponseCaching();
            var containerBuilder = new ContainerBuilder();
            var infrastructureModule = new InfrastructureModule(
                Configuration.GetConnectionString("DefaultConnection"),
                Configuration["AzureSearch:SearchServiceName"],
                Configuration["AzureSearch:SearchServiceAdminApiKey"],
                null,
                Infrastructure.Environment.Web
                );
            containerBuilder.RegisterModule(infrastructureModule);
            //containerBuilder.RegisterModule<>();
            containerBuilder.Populate(services);
            var container = containerBuilder.Build();
            return new AutofacServiceProvider(container);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            var supportedCultures = new[]
            {
                new CultureInfo("en-US")
            };
            app.UseRequestLocalization(new RequestLocalizationOptions()
            {
                DefaultRequestCulture = new RequestCulture(supportedCultures[0]),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures
            });
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions
                {
                    HotModuleReplacement = true
                });
            }
            else
            {

                //     app.UseExceptionHandler("Home/Error");
            }
            app.UseRewriter(new RewriteOptions().AddRedirectToHttpsPermanent());

            app.UseResponseCompression();
            app.UseResponseCaching();
            app.UseStatusCodePages();
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor
                | Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto
            });
            app.UseStaticFiles(new StaticFileOptions
            {
                OnPrepareResponse = ctx => ctx.Context.Response.Headers.Add("Cache-Control", "public,max-age=600")
            });
            //app.AddResponseCompression()
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

                routes.MapSpaFallbackRoute(
                    name: "spa-fallback",
                    defaults: new { controller = "Home", action = "Index" });
            });
        }
    }
}
