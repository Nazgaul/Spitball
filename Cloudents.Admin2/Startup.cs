using Autofac;
using Autofac.Extensions.DependencyInjection;
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
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Cloudents.Admin2
{
    //Client secret from azure: JPJAvY3Dk]q:EsGA]5REfUt*bkDAuy51
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

            services.AddAuthorization();
            services.AddAuthentication(o =>
                {
                    o.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                })
                
                .AddCookie(x =>
                {
                    x.Events.OnRedirectToLogin = context =>
                    {
                        if (context.Request.Path.StartsWithSegments("/api"))
                        {
                            context.Response.StatusCode = 401;
                            return Task.CompletedTask;
                        }
                        else
                        {
                            context.Response.Redirect("/account/LogIn"); 
                            return Task.CompletedTask;
                        }
                      
                    };
                    x.Events.OnRedirectToAccessDenied = context =>
                    {
                        context.Response.StatusCode = 403;
                        return Task.CompletedTask;
                    };
                });

          

            services.AddDataProtection(o =>
            {
                o.ApplicationDiscriminator = "spitball";
            }).PersistKeysToAzureBlobStorage(CloudStorageAccount.Parse(Configuration["Storage"]), "/spitball/keys/keys.xml");
            services.AddResponseCompression();
            services.AddResponseCaching();

            //services.AddLocalization(x => x.ResourcesPath = "Resources");
            services.AddMvc(config =>
            {
               
                    var policy = new AuthorizationPolicyBuilder()
                        .RequireAuthenticatedUser()
                        .Build();
                    config.Filters.Add(new AuthorizeFilter(policy));
            }).AddJsonOptions(options =>
                {
                    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                    options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                    options.SerializerSettings.Converters.Add(new StringEnumConverter { CamelCaseText = true });
                })
                .AddDataAnnotationsLocalization()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            if (HostingEnvironment.IsDevelopment())
            {
                SwaggerInitial(services);
            }

            services.AddHttpClient();
            services.AddOptions();
            services.Configure<PayMeCredentials>(Configuration.GetSection("PayMe"));

            services.AddAuthorization(options =>
            {
               // options.AddPolicy(Policy.IsraelUser, policy => policy.RequireClaim(ClaimsPrincipalExtensions.ClaimCountry, "IL"));
               // options.AddPolicy(Policy.IndiaUser, policy => policy.RequireClaim(ClaimsPrincipalExtensions.ClaimCountry, "IN"));
               // options.AddPolicy(Policy.GlobalUser, policy => policy.RequireClaim(ClaimsPrincipalExtensions.ClaimCountry, "None"));
            });

            var assembliesOfProgram = new[]
            {
                Assembly.Load("Cloudents.Core"),
                Assembly.Load("Cloudents.Infrastructure.Storage"),
                Assembly.Load("Cloudents.Infrastructure"),
                Assembly.Load("Cloudents.Persistence"),
                Assembly.Load("Cloudents.Search"),
                Assembly.GetExecutingAssembly()
            };

            var containerBuilder = new ContainerBuilder();
            containerBuilder.Register(c =>
            {
                var val = c.Resolve<IOptionsMonitor<PayMeCredentials>>();
                return val.CurrentValue;
            }).AsSelf();
            var keys = new ConfigurationKeys()
            {
                SiteEndPoint = { SpitballSite = Configuration["Site"] },
                Db = new DbConnectionString(Configuration.GetConnectionString("DefaultConnection"), Configuration["ReadDb"], Configuration["Redis"]),
                Search = new SearchServiceCredentials(Configuration["AzureSearch:SearchServiceName"],
                       Configuration["AzureSearch:SearchServiceAdminApiKey"],
                    !HostingEnvironment.IsProduction()
                    ),
                Storage = Configuration["Storage"],
                Redis = Configuration["Redis"],
                ServiceBus = Configuration["ServiceBus"]
            };

            containerBuilder.Register(_ => keys).As<IConfigurationKeys>();
            containerBuilder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly()).AsClosedTypesOf(typeof(IEventHandler<>));
            containerBuilder.RegisterAssemblyModules(assembliesOfProgram);
            containerBuilder.Register(c =>
            {
                var z = c.Resolve<IHttpClientFactory>();
                return z.CreateClient();
            });
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

            app.UseResponseCompression();
            app.UseResponseCaching();

            app.UseHttpsRedirection();
            app.UseStaticFiles();
           
            app.UseCookiePolicy();
            app.UseAuthentication();

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
