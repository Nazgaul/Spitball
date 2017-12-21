﻿using System;
using System.Collections.Generic;
using System.Globalization;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Cloudents.Core.DTOs;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Request;
using Cloudents.Infrastructure;
using Cloudents.Infrastructure.Data;
using Cloudents.Web.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Newtonsoft.Json.Converters;
using WebMarkupMin.AspNetCore2;

namespace Cloudents.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
            HostingEnvironment = env;
        }

        public IConfiguration Configuration { get; }
        private IHostingEnvironment HostingEnvironment { get;  }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddWebMarkupMin().AddHtmlMinification();
            services.AddLocalization(options => options.ResourcesPath = "Resources");
            services.AddMvc()
                .AddJsonOptions(options =>
            {
                options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
                options.SerializerSettings.Converters.Add(new StringEnumConverter { CamelCaseText = true });
                options.SerializerSettings.Converters.Add(new IsoDateTimeConverter
                {
                    DateTimeStyles = DateTimeStyles.AssumeUniversal
                });
            });
            services.Configure<MvcOptions>(options =>
            {
                options.Filters.Add(new RequireHttpsAttribute());
            });
            services.AddResponseCompression();
            services.AddResponseCaching();

            var connection = Configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(connection);
            });

            
            var physicalProvider = HostingEnvironment.ContentRootFileProvider;
            services.AddSingleton(physicalProvider);

            var containerBuilder = new ContainerBuilder();
            services.AddSingleton<WebPackChunkName>();
            var infrastructureModule = new WebInfrastructureModule(
                Configuration.GetConnectionString("DefaultConnection"),
                Configuration["AzureSearch:SearchServiceName"],
                Configuration["AzureSearch:SearchServiceAdminApiKey"],
                Configuration["Redis"]
                );
            containerBuilder.RegisterModule(infrastructureModule);

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
                OnPrepareResponse = ctx =>
                {
                    ctx.Context.Response.Headers.Add("Cache-Control", "public,max-age=864000");
                    ctx.Context.Response.Headers.Add("Access-Control-Allow-Origin","*");
                }
            });
            app.UseWebMarkupMin();
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
