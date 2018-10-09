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
using Cloudents.Web.Hubs;
using Cloudents.Web.Identity;
using Cloudents.Web.Middleware;
using Cloudents.Web.Services;
using JetBrains.Annotations;
using Joonasw.AspNetCore.SecurityHeaders;
using Microsoft.ApplicationInsights.AspNetCore;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.SnapshotCollector;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.WindowsAzure.Storage;
using Newtonsoft.Json;
using System;
using System.Globalization;
using System.Reflection;
using System.Threading.Tasks;
using WebMarkupMin.AspNetCore2;
using Logger = Cloudents.Web.Services.Logger;

namespace Cloudents.Web
{
    public partial class Startup
    {
        public const string IntegrationTestEnvironmentName = "Integration-Test";
        internal const int PasswordRequiredLength = 8;

        public static readonly CultureInfo[] SupportedCultures = {

            new CultureInfo("en"),
            new CultureInfo("he"),
        };

        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
            HostingEnvironment = env;
        }

        private IConfiguration Configuration { get; }
        private IHostingEnvironment HostingEnvironment { get; }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to conapp\Cloudents.Web\Startup.csfigure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940

        [UsedImplicitly]
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            // Configure SnapshotCollector from application settings
            services.Configure<SnapshotCollectorConfiguration>(Configuration.GetSection(nameof(SnapshotCollectorConfiguration)));
            // Add SnapshotCollector telemetry processor.
            services.AddSingleton<ITelemetryProcessorFactory>(sp => new SnapshotCollectorTelemetryProcessorFactory(sp));
            services.AddSingleton<ITelemetryInitializer, RequestBodyInitializer>();
            services.AddSingleton<ITelemetryInitializer, UserIdInitializer>();

            services.AddLocalization(x => x.ResourcesPath = "Resources");
            services.AddDataProtection(o =>
            {
                o.ApplicationDiscriminator = "spitball";
            }).PersistKeysToAzureBlobStorage(CloudStorageAccount.Parse(Configuration["Storage"]), "/spitball/keys/keys.xml");
            //services.Configure<CookiePolicyOptions>(options =>
            //{
            //    // This lambda determines whether user consent for non-essential cookies is needed for a given request.
            //    options.CheckConsentNeeded = context => true;
            //    options.MinimumSameSitePolicy = SameSiteMode.None;
            //});

            services.AddWebMarkupMin().AddHtmlMinification();
            services.AddMvc()
                .AddMvcLocalization(LanguageViewLocationExpanderFormat.SubFolder, o =>
                {
                    o.DataAnnotationLocalizerProvider = (type, factory) =>
                    {
                        var assemblyName = new AssemblyName(typeof(DataAnnotationSharedResource).GetTypeInfo().Assembly.FullName);
                        return factory.Create("DataAnnotationSharedResource", assemblyName.Name);
                    };
                })
                .AddCookieTempDataProvider(o =>
                {
                    o.Cookie.Name = "td";
                    o.Cookie.HttpOnly = true;
                })
                .AddJsonOptions(options =>
            {
                options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                options.SerializerSettings.Converters.Add(new StringEnumNullUnknownStringConverter { CamelCaseText = true });
                options.SerializerSettings.Converters.Add(new RequestCultureConverter());
                options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
            })

                .AddMvcOptions(o =>
                {
                    //TODO: check in source code
                    // o.SuppressBindingUndefinedValueToEnumType
                    o.Filters.Add<UserLockedExceptionFilter>();
                    o.Filters.Add(new GlobalExceptionFilter());
                    o.Filters.Add(new ResponseCacheAttribute
                    {
                        NoStore = true,
                        Location = ResponseCacheLocation.None
                    });
                    o.ModelBinderProviders.Insert(0, new ApiBinder());
                }).SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            if (HostingEnvironment.IsDevelopment())
            {
                Swagger.Startup.SwaggerInitial(services);
            }

            services.AddSignalR().AddRedis(Configuration["Redis"]).AddJsonProtocol(o =>
                {
                    o.PayloadSerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                    o.PayloadSerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                    o.PayloadSerializerSettings.Converters.Add(new StringEnumNullUnknownStringConverter { CamelCaseText = true });
                });
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

                options.Password.RequiredLength = PasswordRequiredLength;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredUniqueChars = 0;
                options.Lockout.MaxFailedAccessAttempts = 3;


            }).AddDefaultTokenProviders().AddSignInManager<SbSignInManager>();
            //services.Configure<SecurityStampValidatorOptions>(o =>
            //{
            //    o.ValidationInterval = TimeSpan.FromMinutes(2);
            //});
            
            services.ConfigureApplicationCookie(o =>
            {
                o.Cookie.Name = "sb4";
                o.SlidingExpiration = true;

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
                Assembly.GetExecutingAssembly()
            };
            services.AddAutoMapper(c => c.DisableConstructorMapping(), assembliesOfProgram);
            var containerBuilder = new ContainerBuilder();
            services.AddSingleton<WebPackChunkName>();
            var keys = new ConfigurationKeys(Configuration["Site"])
            {
                Db = new DbConnectionString(Configuration.GetConnectionString("DefaultConnection"), Configuration["Redis"]),
                Redis = Configuration["Redis"],
                Search = new SearchServiceCredentials(Configuration["AzureSearch:SearchServiceName"],
                       Configuration["AzureSearch:SearchServiceAdminApiKey"],
                    !HostingEnvironment.IsProduction()
                    ),
                Storage = Configuration["Storage"],
                BlockChainNetwork = Configuration["BlockChainNetwork"],
                ServiceBus = Configuration["ServiceBus"]
            };

            containerBuilder.Register(_ => keys).As<IConfigurationKeys>();
            containerBuilder.RegisterSystemModules(
                Core.Enum.System.Web, assembliesOfProgram);
            containerBuilder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly()).AsClosedTypesOf(typeof(IEventHandler<>));
            containerBuilder.RegisterType<Logger>().As<ILogger>();
            containerBuilder.Populate(services);
            var container = containerBuilder.Build();
            return new AutofacServiceProvider(container);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        [UsedImplicitly]
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseHeaderRemover("X-HTML-Minification-Powered-By");
            app.UseClickJacking();

            // BuildCsp(app);

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
                app.UseHsts(new HstsOptions
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
            

            app.UseRequestLocalization(o =>
            {

                o.DefaultRequestCulture = new RequestCulture(SupportedCultures[0]);
                // Formatting numbers, dates, etc.
                o.SupportedCultures = SupportedCultures;
                // UI strings that we have localized.
                o.SupportedUICultures = SupportedCultures;
                o.RequestCultureProviders.Add(new AuthorizedUserCultureProvider());

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
            if (env.IsDevelopment() /*|| env.IsStaging()*/)
            {
                app.UseSwagger();
                // Enable middleWare to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1"));
            }

            app.UseAuthentication();
            
            app.UseSignalR(routes =>
            {
                routes.MapHub<SbHub>("/SbHub");
            });
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

        private static void BuildCsp(IApplicationBuilder app)
        {
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
                    .From("*.talkjs.com")
                    .From("https://connect.facebook.net/en_US/fbevents.js")
                    .From("https://connect.facebook.net/signals/config/1770276176567240");


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
                    .From("https://www.googletagmanager.com")
                    .From("https://www.facebook.com/tr/")
                    .From("https://stats.g.doubleclick.net/r/collect")
                    .From("https://www.google.com/ads/ga-audiences")
                    .From("https://www.google.co.il/ads/ga-audiences");

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
        }
    }
}
