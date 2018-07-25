using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutoMapper;
using Cloudents.Core;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Extension;
using Cloudents.Core.Interfaces;
using Cloudents.Web.Binders;
using Cloudents.Web.Extensions;
using Cloudents.Web.Filters;
using Cloudents.Web.Identity;
using Cloudents.Web.Middleware;
using Cloudents.Web.Services;
using Cloudents.Web.Swagger;
using JetBrains.Annotations;
using Microsoft.ApplicationInsights.AspNetCore;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.SnapshotCollector;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Swashbuckle.AspNetCore.Swagger;
using WebMarkupMin.AspNetCore2;
using Logger = Cloudents.Web.Services.Logger;

namespace Cloudents.Web
{
    public class Startup
    {
        public const string IntegrationTestEnvironmentName = "Integration-Test";

        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
            HostingEnvironment = env;
        }

        private IConfiguration Configuration { get; }
        private IHostingEnvironment HostingEnvironment { get; }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940

        [UsedImplicitly]
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            // Configure SnapshotCollector from application settings
            services.Configure<SnapshotCollectorConfiguration>(Configuration.GetSection(nameof(SnapshotCollectorConfiguration)));

            // Add SnapshotCollector telemetry processor.
            services.AddSingleton<ITelemetryProcessorFactory>(sp => new SnapshotCollectorTelemetryProcessorFactory(sp));
            services.AddSingleton<ITelemetryInitializer, RequestBodyInitializer>();

            services.AddWebMarkupMin().AddHtmlMinification();
            services.AddMvc()
                .AddJsonOptions(options =>
            {
                options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                options.SerializerSettings.Converters.Add(new StringEnumConverter { CamelCaseText = true });
                options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
            }).AddMvcOptions(o =>
                {
                    o.Filters.Add(new GlobalExceptionFilter());
                    o.ModelBinderProviders.Insert(0, new ApiBinder()); //needed at home
                });
            if (HostingEnvironment.IsDevelopment())
            {
                services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new Info { Title = "Spitball Api", Version = "v1" });
                    var basePath = AppContext.BaseDirectory;
                    var xmlPath = Path.Combine(basePath, "Cloudents.Web.xml");
                    c.IncludeXmlComments(xmlPath);
                    c.DescribeAllEnumsAsStrings();
                    c.DescribeAllParametersInCamelCase();
                    c.OperationFilter<FormFileOperationFilter>();
                    c.ResolveConflictingActions(f =>
                    {
                        var descriptions = f.ToList();
                        var parameters = descriptions
                            .SelectMany(desc => desc.ParameterDescriptions)
                            .GroupBy(x => x, (x, xs) => new { IsOptional = xs.Count() == 1, Parameter = x },
                                ApiParameterDescriptionEqualityComparer.Instance)
                            .ToList();
                        var description = descriptions[0];
                        description.ParameterDescriptions.Clear();
                        parameters.ForEach(x =>
                        {
                            if (x.Parameter.RouteInfo != null)
                                x.Parameter.RouteInfo.IsOptional = x.IsOptional;
                            description.ParameterDescriptions.Add(x.Parameter);
                        });
                        return description;
                    });
                });
            }

            services.AddResponseCompression();
            services.AddResponseCaching();

            var physicalProvider = HostingEnvironment.ContentRootFileProvider;
            services.AddSingleton(physicalProvider);

            services.AddScoped<SignInManager<User>, SbSignInManager>();
            services.AddIdentity<User, ApplicationRole>(options =>
            {
                options.SignIn.RequireConfirmedEmail = true;
                options.SignIn.RequireConfirmedPhoneNumber = true;
                options.User.AllowedUserNameCharacters = null;

                options.User.RequireUniqueEmail = true;

                options.Password.RequiredLength = 1;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredUniqueChars = 0;
            }).AddDefaultTokenProviders().AddSignInManager<SbSignInManager>();

            services.AddAuthorization();
            services.ConfigureApplicationCookie(o =>
            {
                o.Cookie.Name = "sb2";
                o.Events.OnRedirectToLogin = context =>
                {
                    context.Response.StatusCode = 401;
                    return Task.CompletedTask;
                };
                o.Events.OnRedirectToAccessDenied = context =>
                {
                    context.Response.StatusCode = 401;
                    return Task.CompletedTask;
                };
            });
            services.AddAuthentication();

            services.AddScoped<IUserClaimsPrincipalFactory<User>, AppClaimsPrincipalFactory>();
            services.AddTransient<IUserStore<User>, UserStore>();
            services.AddTransient<IRoleStore<ApplicationRole>, RoleStore>();
            services.AddTransient<ISmsSender, SmsSender>();
            
            var assembliesOfProgram = new[]
            {
                Assembly.Load("Cloudents.Infrastructure.Framework"),
                Assembly.Load("Cloudents.Infrastructure.Storage"),
                Assembly.Load("Cloudents.Infrastructure"),
                Assembly.Load("Cloudents.Core"),
                Assembly.Load("Cloudents.Infrastructure.Data"),
                Assembly.GetExecutingAssembly()
            };
            services.AddAutoMapper(c => c.DisableConstructorMapping(), assembliesOfProgram);

            var containerBuilder = new ContainerBuilder();
            services.AddSingleton<WebPackChunkName>();

            var keys = new ConfigurationKeys
            {
                Db = Configuration.GetConnectionString("DefaultConnection"),
                Search = new SearchServiceCredentials(Configuration["AzureSearch:SearchServiceName"],
                       Configuration["AzureSearch:SearchServiceAdminApiKey"]),
                Redis = Configuration["Redis"],
                Storage = Configuration["Storage"],
                //FunctionEndpoint = Configuration["AzureFunction:EndPoint"],
                BlockChainNetwork = Configuration["BlockChainNetwork"],
                ServiceBus = Configuration["ServiceBus"]
            };

            containerBuilder.Register(_ => keys).As<IConfigurationKeys>();
            containerBuilder.RegisterSystemModules(
                Core.Enum.System.Web, assembliesOfProgram);

            containerBuilder.RegisterType<Logger>().As<ILogger>();
            //containerBuilder.RegisterType<UrlConst>().As<IUrlBuilder>().SingleInstance();
            containerBuilder.Populate(services);
            var container = containerBuilder.Build();
            return new AutofacServiceProvider(container);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        [UsedImplicitly]
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseHeaderRemover("X-HTML-Minification-Powered-By");

            if (env.IsDevelopment())
            {
                HibernatingRhinos.Profiler.Appender.NHibernate.NHibernateProfiler.Initialize();
                app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions
                {
                    HotModuleReplacement = true
                });
                var configuration = app.ApplicationServices.GetService<TelemetryConfiguration>();
               // configuration.DisableTelemetry = true;

            }
            var reWriterOptions = new RewriteOptions()
                .Add(new RemoveTrailingSlash());

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseStatusCodePagesWithReExecute("/Error");
                app.UseExceptionHandler("/Error");
            }

            if (!env.IsDevelopment() && !env.IsEnvironment(IntegrationTestEnvironmentName))
            {
                reWriterOptions.AddRedirectToHttpsPermanent();
            }

            app.UseRewriter(reWriterOptions);

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
                    ctx.Context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
                }
            });

            app.UseWebMarkupMin();
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1"));
            }
            app.UseAuthentication();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
            app.MapWhen(x => !x.Request.Path.Value.StartsWith("/api"), builder =>
            {
                builder.UseMvc(routes =>
                {
                    routes.MapSpaFallbackRoute(
                        name: "spa-fallback",
                        defaults: new { controller = "Home", action = "Index" });
                });
            });
        }

        private class SnapshotCollectorTelemetryProcessorFactory : ITelemetryProcessorFactory
        {
            private readonly IServiceProvider _serviceProvider;

            public SnapshotCollectorTelemetryProcessorFactory(IServiceProvider serviceProvider) =>
                _serviceProvider = serviceProvider;

            public ITelemetryProcessor Create(ITelemetryProcessor next)
            {
                var snapshotConfigurationOptions = _serviceProvider.GetService<IOptions<SnapshotCollectorConfiguration>>();
                return new SnapshotCollectorTelemetryProcessor(next, configuration: snapshotConfigurationOptions.Value);
            }
        }
    }

    public class UserTelemetryInitializer : ITelemetryInitializer
    {
        public void Initialize(ITelemetry telemetry)
        {
            throw new NotImplementedException();
        }
    }
}
