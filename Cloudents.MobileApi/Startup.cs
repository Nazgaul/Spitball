using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Cloudents.Core;
using Cloudents.Core.Interfaces;
using Cloudents.Infrastructure;
using Cloudents.Infrastructure.Framework;
using Cloudents.MobileApi.Filters;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Converters;
using Swashbuckle.AspNetCore.Swagger;

namespace Cloudents.MobileApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().AddMvcOptions(o =>
            {
                o.ModelBinderProviders.Insert(0, new LocationModelBinder());
                o.ModelBinderProviders.Insert(0, new GeoPointModelBinder());
            }).AddJsonOptions(options =>
            {
                options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
                options.SerializerSettings.Converters.Add(new StringEnumConverter { CamelCaseText = false });
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
            var containerBuilder = new ContainerBuilder();
            var keys = new ConfigurationKeys
            {
                Db = Configuration.GetConnectionString("DefaultConnection"),
                Search = new SearchServiceCredentials(Configuration["AzureSearch:SearchServiceName"],
                    Configuration["AzureSearch:SearchServiceAdminApiKey"]),
                Redis = Configuration["Redis"],
                Storage = Configuration["Storage"],
                SystemUrl = Configuration["SystemUrl"]
            };
            containerBuilder.Register(_ => keys).As<IConfigurationKeys>();
            containerBuilder.RegisterModule<ModuleMobile>();
            containerBuilder.RegisterModule<ModuleDb>();
            containerBuilder.Populate(services);
            var container = containerBuilder.Build();
            return new AutofacServiceProvider(container);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            //if (env.IsDevelopment())
            //{
                app.UseDeveloperExceptionPage();
            //}

            app.UseStaticFiles();
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
