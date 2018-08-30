using System;
using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Cloudents.Core;
using Cloudents.Core.Extension;
using Cloudents.Core.Interfaces;
using Cloudents.Management.Extensions;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.WindowsAzure.Storage;

namespace Cloudents.Management
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
            services.AddAuthentication(sharedOptions =>
            {
                sharedOptions.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                sharedOptions.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddAzureAd(options => Configuration.Bind("AzureAd", options))
            .AddCookie();

            services.AddDataProtection(o =>
            {
                o.ApplicationDiscriminator = "spitball";
            }).PersistKeysToAzureBlobStorage(CloudStorageAccount.Parse(Configuration["Storage"]), "/spitball/keys/keys.xml");
            services.AddMvc(config =>
                {
                    var policy = new AuthorizationPolicyBuilder()
                        .RequireAuthenticatedUser()
                        .Build();
                    config.Filters.Add(new AuthorizeFilter(policy));
                })
                //.AddMvcOptions(o=>o.Filters.Add(new AuthorizeFilter()))
                .AddRazorPagesOptions(o => o.RootDirectory = "/Pages");


            var assembliesOfProgram = new[]
            {
                Assembly.Load("Cloudents.Infrastructure.Framework"),
                Assembly.Load("Cloudents.Core"),
                Assembly.Load("Cloudents.Infrastructure.Storage"), 
                Assembly.Load("Cloudents.Infrastructure"), 
                Assembly.GetExecutingAssembly()
            };

            var containerBuilder = new ContainerBuilder();

            var keys = new ConfigurationKeys(Configuration["Site"])
            {
                Db = new DbConnectionString(Configuration.GetConnectionString("DefaultConnection"), Configuration["Redis"]),
                ServiceBus = Configuration["ServiceBus"],
                Storage =  Configuration["Storage"],
                Redis = Configuration["Redis"]
            };

            containerBuilder.Register(_ => keys).As<IConfigurationKeys>();
            containerBuilder.RegisterSystemModules(
                Core.Enum.System.Admin, assembliesOfProgram);

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

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
