using System;
using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Cloudents.Core;
using Cloudents.Core.Extension;
using Cloudents.Core.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Cloudents.Ico
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
            services.AddMvc();


            var containerBuilder = new ContainerBuilder();
            var assembliesOfProgram = new[]
            {
                Assembly.Load("Cloudents.Infrastructure.Framework"),
                //Assembly.Load("Cloudents.Infrastructure.Storage"),
                //Assembly.Load("Cloudents.Infrastructure"),
                Assembly.Load("Cloudents.Core"),
                //Assembly.Load("Cloudents.Infrastructure.Data"),
                Assembly.GetExecutingAssembly()
            };
            var keys = new ConfigurationKeys
            {
                //Db = Configuration.GetConnectionString("DefaultConnection"),
                //Search = new SearchServiceCredentials(Configuration["AzureSearch:SearchServiceName"],
                //    Configuration["AzureSearch:SearchServiceAdminApiKey"]),
                //Redis = Configuration["Redis"],
                //Storage = Configuration["Storage"],
                //FunctionEndpoint = Configuration["AzureFunction:EndPoint"],
                //BlockChainNetwork = Configuration["BlockChainNetwork"],
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
            }

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
