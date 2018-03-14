using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Cloudents.Api.Binders;
using Cloudents.Core;
using Cloudents.Core.Interfaces;
using Cloudents.Infrastructure;
using Cloudents.Infrastructure.Framework;
using Cloudents.Infrastructure.Storage;
using Cloudents.Web.Extensions.Binders;
using Cloudents.Web.Extensions.Filters;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Converters;
using Swashbuckle.AspNetCore.Swagger;

namespace Cloudents.Api
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
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolity", builder => 
                        builder.SetIsOriginAllowed(origin =>
                    {
                        if (origin.IndexOf("localhost", StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            return true;
                        }

                        if (origin.IndexOf("spitball", StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            return true;
                        }
                        return false;
                    })
                );
            });
            services.AddMvc().AddMvcOptions(o =>
            {
                o.Filters.Add(new GlobalExceptionFilter(HostingEnvironment));
                o.ModelBinderProviders.Insert(0, new LocationModelBinder());
                o.ModelBinderProviders.Insert(0, new GeoPointModelBinder());
            }).AddJsonOptions(options =>
            {
                options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
                options.SerializerSettings.Converters.Add(new StringEnumConverter { CamelCaseText = true });
                options.SerializerSettings.Converters.Add(new PrioritySourceJsonConverter());
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

            containerBuilder.RegisterSystemModules(
                Core.Enum.System.Api,
                Assembly.Load("Cloudents.Infrastructure.Framework"),
                Assembly.Load("Cloudents.Infrastructure.Storage"),
                Assembly.Load("Cloudents.Infrastructure"),
                Assembly.Load("Cloudents.Core"));
            
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
            app.UseCors("CorsPolity");
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

           
            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            app.UseMvc();
        }
    }
}
