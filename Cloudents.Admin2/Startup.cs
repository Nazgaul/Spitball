using Autofac;
using Autofac.Extensions.DependencyInjection;
using Cloudents.Admin2.Extensions;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.WindowsAzure.Storage;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.IO;
using System.Reflection;
using Cloudents.Core;
using Cloudents.Core.Interfaces;

namespace Cloudents.Admin2
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
            HostingEnvironment = env;
        }

        public IConfiguration Configuration { get; }
        private IHostingEnvironment HostingEnvironment { get; }

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

            services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin",
                    builder => builder.WithOrigins("https://login.microsoftonline.com"));
            });


            services.AddMvc(config =>
            {
                if (!HostingEnvironment.IsDevelopment())
                {
                    var policy = new AuthorizationPolicyBuilder()
                        .RequireAuthenticatedUser()
                        .Build();
                    config.Filters.Add(new AuthorizeFilter(policy));
                }

            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            if (HostingEnvironment.IsDevelopment())
            {
                SwaggerInitial(services);
            }
            var assembliesOfProgram = new[]
            {
                Assembly.Load("Cloudents.Core"),
                Assembly.Load("Cloudents.Infrastructure.Storage"),
                Assembly.Load("Cloudents.Infrastructure"),
                Assembly.Load("Cloudents.Persistance"),
                Assembly.Load("Cloudents.Search"),
                Assembly.GetExecutingAssembly()
            };

            var containerBuilder = new ContainerBuilder();

            var keys = new ConfigurationKeys(Configuration["Site"])
            {
                Db = new DbConnectionString(Configuration.GetConnectionString("DefaultConnection"), Configuration["Redis"]),
                Search = new SearchServiceCredentials(Configuration["AzureSearch:SearchServiceName"],
                       Configuration["AzureSearch:SearchServiceAdminApiKey"],
                    !HostingEnvironment.IsProduction()
                    ),
                Storage = Configuration["Storage"],
                Redis = Configuration["Redis"],
                ServiceBus = Configuration["ServiceBus"]
            };

            containerBuilder.Register(_ => keys).As<IConfigurationKeys>();

            //containerBuilder.RegisterSystemModules(
            //    Application.Enum.System.Admin, assembliesOfProgram);
            containerBuilder.RegisterAssemblyModules(assembliesOfProgram);
            containerBuilder.Populate(services);
            var container = containerBuilder.Build();
            return new AutofacServiceProvider(container);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions
                {
                    HotModuleReplacement = true
                });
                app.UseSwagger();
                // Enable middleWare to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1"));
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");

            }

            app.UseCors("AllowSpecificOrigin");

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            if (!env.IsDevelopment())
            {
                app.UseAuthentication();
            }

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


        private static void SwaggerInitial(IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Admin Api", Version = "v1" });
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
                c.DescribeAllEnumsAsStrings();
                c.DescribeAllParametersInCamelCase();
            });
        }
    }
}
