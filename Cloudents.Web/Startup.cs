﻿using System;
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
using Microsoft.AspNetCore.Mvc;
using Joonasw.AspNetCore.SecurityHeaders;
using Microsoft.AspNetCore.Http;
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


            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddWebMarkupMin().AddHtmlMinification();
            services.AddMvc()
                .AddCookieTempDataProvider(o =>
                {
                    o.Cookie.Name = "td";
                    o.Cookie.HttpOnly = true;
                })
                .AddJsonOptions(options =>
            {
                options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                options.SerializerSettings.Converters.Add(new StringEnumConverter { CamelCaseText = true });
                options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
            }).AddMvcOptions(o =>
                {

                    o.Filters.Add(new GlobalExceptionFilter());
                    o.Filters.Add(new ResponseCacheAttribute
                    {
                        NoStore = true,
                        Location = ResponseCacheLocation.None
                    });
                    o.ModelBinderProviders.Insert(0, new ApiBinder()); //needed at home
                }).SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            if (HostingEnvironment.IsDevelopment())
            {
                SwaggerInitial(services);
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

            var keys = new ConfigurationKeys(Configuration["Site"])
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

        private static void SwaggerInitial(IServiceCollection services)
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

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        [UsedImplicitly]
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseHeaderRemover("X-HTML-Minification-Powered-By");
            app.UseClickJacking();

            app.UseCsp(csp =>
            {
                // If nothing is mentioned for a resource class, allow from this domain
                csp.ByDefaultAllow
                    .FromSelf();


                // Allow JavaScript from:
                csp.AllowScripts.FromSelf().AllowUnsafeEval().AllowUnsafeInline()
                    .From("https://app.intercom.io")
                    .From("https://widget.intercom.io")
                    .From("https://js.intercomcdn.com")
                    .From("https://www.google-analytics.com/")
                    .From("https://www.googletagmanager.com/")
                    .From("https://googleads.g.doubleclick.net")
                    .From("https://bid.g.doubleclick.net")
                    .From("https://www.googleadservices.com")
                    .From("*.google.com")
                    .From("https://www.gstatic.com/")
                    .From("*.inspectlet.com")
                    .From("*.talkjs.com");


                //csp.AllowScripts.FromSelf().AllowUnsafeInline().AllowUnsafeEval()
                //    .From("www.google-analytics.com")
                //    .From("*.google.com").From("*.googletagmanager.com")
                //    .From("*.gstatic.com").From("*.talkjs.com");

                // CSS allowed from:
                csp.AllowStyles.FromSelf().AllowUnsafeInline()
                    .From("https://fonts.googleapis.com");

                //image files
                csp.AllowImages.FromSelf()
                    .From("data:")
                    .From("https://js.intercomcdn.com")
                    .From("https://static.intercomassets.com")
                    .From("https://downloads.intercomcdn.com")
                    .From("https://uploads.intercomusercontent.com")
                    .From("https://gifs.intercomcdn.com")
                    .From("https://www.google-analytics.com/")
                    .From("*.talkjs.com")
                    .From("https://www.googletagmanager.com");

                // Contained iframes can be sourced from:
                csp.AllowFrames
                    .From("https://share.intercom.io")
                    .From("https://intercom-sheets.com")
                    .From("https://www.youtube.com")
                    .From("https://player.vimeo.com")
                    .From("https://fast.wistia.net")
                    .From("https://www.googletagmanager.com/ns.html")
                    .From("https://www.google.com/recaptcha/")
                    .From("*.inspectlet.com");


                csp.AllowWorkers
                    .From("https://share.intercom.io")
                    .From("https://intercom-sheets.com")
                    .From("https://www.youtube.com")
                    .From("https://player.vimeo.com")
                    .From("https://fast.wistia.net")
                    .From("*.inspectlet.com")
                    .From("*.talkjs.com");

                //media files
                csp.AllowAudioAndVideo.From("https://js.intercomcdn.com")
                    .From("*.inspectlet.com")
                    .From("*.talkjs.com");


                // Allow AJAX, WebSocket and EventSource connections to:
                csp.AllowConnections.ToSelf()
                .To("https://api.intercom.io")
                .To("https://api-iam.intercom.io")
                .To("https://api-ping.intercom.io")
                .To("https://nexus-websocket-a.intercom.io")
                .To("https://nexus-websocket-b.intercom.io")
                .To("https://nexus-long-poller-a.intercom.io")
                .To("https://nexus-long-poller-b.intercom.io")
                .To("wss://nexus-websocket-a.intercom.io")
                .To("wss://nexus-websocket-b.intercom.io")
                .To("https://uploads.intercomcdn.com")
                .To("https://uploads.intercomusercontent.com")
                .To("https://app.getsentry.com")
                    .To("https://www.google-analytics.com/")
                    .To("*.inspectlet.com")
                    .To("*.talkjs.com");


                // Allow fonts to be downloaded from:
                csp.AllowFonts.FromSelf()
                    .From("data:")
                    .From("https://fonts.gstatic.com")
                    .From("https://js.intercomcdn.com");

                // Allow object, embed, and applet sources from:
                csp.ByDefaultAllow.FromNowhere();

                // Allow other sites to put this in an iframe?
                csp.AllowFraming
                    .FromNowhere(); // Block framing on other sites, equivalent to X-Frame-Options: DENY

                csp.SetReportOnly();
                csp.ReportViolationsTo("api/report/csp");

                // Do not include the CSP header for requests to the /api endpoints
                csp.OnSendingHeader = context =>
                {
                    context.ShouldNotSend = context.HttpContext.Request.Path.StartsWithSegments("/api");
                    return Task.CompletedTask;
                };
            });

            if (env.IsDevelopment())
            {
                //HibernatingRhinos.Profiler.Appender.NHibernate.NHibernateProfiler.Initialize();
                app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions
                {
                    HotModuleReplacement = true
                });
                var configuration = app.ApplicationServices.GetService<TelemetryConfiguration>();
                configuration.DisableTelemetry = true;

                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseStatusCodePagesWithReExecute("/Error");
                app.UseExceptionHandler("/Error");
                app.UseHsts(new HstsOptions()
                {
                    Duration = TimeSpan.FromDays(365),
                    IncludeSubDomains = true,
                    Preload = true
                });
            }
            var reWriterOptions = new RewriteOptions()
                .Add(new RemoveTrailingSlash());
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
                // Enable middleWare to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
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
