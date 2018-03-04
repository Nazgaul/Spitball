using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Cloudents.Core;
using Cloudents.Core.Interfaces;
using Cloudents.Infrastructure;
using Cloudents.Infrastructure.Framework;
using Cloudents.Infrastructure.Storage;
using Cloudents.MobileApi.Binders;
using Cloudents.MobileApi.Filters;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Converters;
using Swashbuckle.AspNetCore.Swagger;

namespace Cloudents.MobileApi
{
    public class Startup
    {
        public const string IntegrationTestEnvironmentName = "Integration-Test";
        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
            HostingEnvironment = env;
        }

        public IConfiguration Configuration { get; }
        private IHostingEnvironment HostingEnvironment { get; }


        // This method gets called by the runtime. Use this method to add services to the container.
        [UsedImplicitly]
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            services.AddMvc().AddMvcOptions(o =>
            {
                o.Filters.Add(new GlobalExceptionFilter(HostingEnvironment));
                o.ModelBinderProviders.Insert(0, new LocationModelBinder());
                o.ModelBinderProviders.Insert(0, new GeoPointModelBinder());
            }).AddJsonOptions(options =>
            {
                options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
                options.SerializerSettings.Converters.Add(new StringEnumConverter { CamelCaseText = true });

                options.SerializerSettings.Converters.Add(new IsoDateTimeConverter
                {
                    DateTimeStyles = DateTimeStyles.AssumeUniversal
                });
            });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "My API", Version = "v1" });
                //Cloudents.MobileApi
                var basePath = AppContext.BaseDirectory;
                var xmlPath = Path.Combine(basePath, "Cloudents.MobileApi.xml");
                c.IncludeXmlComments(xmlPath);
                c.DescribeAllEnumsAsStrings();
            });
            services.AddResponseCompression();
            services.AddResponseCaching();

            var containerBuilder = new ContainerBuilder();
            var keys = new ConfigurationKeys
            {
                Db = Configuration.GetConnectionString("DefaultConnection"),
                Search = new SearchServiceCredentials(Configuration["AzureSearch:SearchServiceName"],
                    Configuration["AzureSearch:SearchServiceAdminApiKey"]),
                Redis = Configuration["Redis"],
                Storage = Configuration["Storage"],
            };
            containerBuilder.Register(_ => keys).As<IConfigurationKeys>();
            containerBuilder.RegisterModule<ModuleMobile>();
            containerBuilder.RegisterModule<ModuleCore>();
            containerBuilder.RegisterModule<ModuleDb>();
            containerBuilder.RegisterModule<ModuleStorage>();
            containerBuilder.Populate(services);
            var container = containerBuilder.Build();
            return new AutofacServiceProvider(container);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        [UsedImplicitly]
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            var reWriterOptions = new RewriteOptions();
                
            if (!env.IsEnvironment(IntegrationTestEnvironmentName))
            {
                reWriterOptions.AddRedirectToHttpsPermanent();
            }

            app.UseRewriter(reWriterOptions);

            app.UseResponseCompression();
            app.UseResponseCaching();
            app.UseDefaultFiles(new DefaultFilesOptions {
                DefaultFileNames = new
                    List<string> { "default.html" }
            });
            app.UseStaticFiles();
            app.UseStatusCodePages();

            app.UseSwagger();

            app.UseCors(builder =>
            {
                //builder.WithOrigins("www.spitball.co", "dev.spitball.co");
                builder.AllowAnyOrigin();
            });
            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            app.UseMvc();
        }
    }
}
