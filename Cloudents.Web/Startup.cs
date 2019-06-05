using Autofac;
using Autofac.Extensions.DependencyInjection;
using Cloudents.Core;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Cloudents.Identity;
using Cloudents.Web.Binders;
using Cloudents.Web.Filters;
using Cloudents.Web.Hubs;
using Cloudents.Web.Identity;
using Cloudents.Web.Middleware;
using Cloudents.Web.Services;
using JetBrains.Annotations;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
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
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Cloudents.Web.Resources;
using Microsoft.Extensions.Options;
using WebMarkupMin.AspNetCore2;
using Logger = Cloudents.Web.Services.Logger;


namespace Cloudents.Web
{
    public class Startup
    {
        public const string IntegrationTestEnvironmentName = "Integration-Test";
        internal const int PasswordRequiredLength = 8;

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
            services.AddSingleton<ITelemetryInitializer, RequestBodyInitializer>();
            services.AddSingleton<ITelemetryInitializer, UserIdInitializer>();

            services.AddLocalization(x => x.ResourcesPath = "Resources");
            services.AddDataProtection(o =>
            {
                o.ApplicationDiscriminator = "spitball";
            }).PersistKeysToAzureBlobStorage(CloudStorageAccount.Parse(Configuration["Storage"]), "/spitball/keys/keys.xml");

            services.AddWebMarkupMin().AddHtmlMinification();
            services.AddRouting(x =>
            {
                // x.ConstraintMap.Add("StorageContainerConstraint", typeof(StorageContainerRouteConstraint));
            });

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
                    o.Filters.Add<OperationCancelledExceptionFilter>();
                    o.Filters.Add<UserLockedExceptionFilter>();
                    o.Filters.Add<GlobalExceptionFilter>();
                    o.Filters.Add(new ResponseCacheAttribute
                    {
                        NoStore = true,
                        Location = ResponseCacheLocation.None
                    });
                    o.ModelBinderProviders.Insert(0, new ApiBinder());
                })
               .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .ConfigureApiBehaviorOptions(o =>
                {
                    o.SuppressMapClientErrors = true; //https://github.com/aspnet/AspNetCore/issues/4792#issuecomment-454164457
                    o.SuppressUseValidationProblemDetailsForInvalidModelStateResponses = false;
                });
            if (HostingEnvironment.IsDevelopment())
            {
                Swagger.Startup.SwaggerInitial(services);
            }

            services.AddSignalR().AddAzureSignalR().AddJsonProtocol(o =>
                {
                    o.PayloadSerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                    o.PayloadSerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                    o.PayloadSerializerSettings.Converters.Add(new StringEnumNullUnknownStringConverter { CamelCaseText = true });
                });
            services.AddResponseCompression();
            services.AddResponseCaching();
            var physicalProvider = HostingEnvironment.ContentRootFileProvider;
            services.AddSingleton(physicalProvider);

            services.AddDetectionCore().AddCrawler();
            services.AddScoped<SignInManager<RegularUser>, SbSignInManager>();
            //RoleStore
            services.AddDefaultIdentity<RegularUser>(options =>
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
            }).AddDefaultTokenProviders()
                .AddClaimsPrincipalFactory<AppClaimsPrincipalFactory>()
                // .AddRoles<RoleStore>()
                .AddSignInManager<SbSignInManager>();
            services.ConfigureApplicationCookie(o =>
            {
                o.Cookie.Name = "sb5";
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


            //TODO: not sure we need those
            services.AddScoped<IUserClaimsPrincipalFactory<RegularUser>, AppClaimsPrincipalFactory>();
            services.AddScoped<IUserStore<RegularUser>, UserStore>();
            //services.AddScoped<IRoleStore<UserRole>, RoleStore>();
            services.AddScoped<ISmsSender, SmsSender>();
            services.AddScoped<ICountryProvider, CountryProvider>();
            services.AddHttpClient();
            services.AddOptions();
            services.Configure<PayMeCredentials>(Configuration.GetSection("PayMe"));
           


            var assembliesOfProgram = new[]
            {
                Assembly.Load("Cloudents.Infrastructure.Storage"),
                Assembly.Load("Cloudents.Infrastructure"),
                Assembly.Load("Cloudents.Core"),
                Assembly.Load("Cloudents.Persistence"),
                Assembly.Load("Cloudents.Search"),
                Assembly.Load("Cloudents.Query"),
                Assembly.GetExecutingAssembly()
            };



            var containerBuilder = new ContainerBuilder();
            containerBuilder.Register(c =>
            {
                var val = c.Resolve<IOptionsMonitor<PayMeCredentials>>();
                return val.CurrentValue;
            }).AsSelf();
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
                ServiceBus = Configuration["ServiceBus"],
                PayPal = new PayPalCredentials(Configuration["PayPal:ClientId"], Configuration["PayPal:ClientSecret"], !HostingEnvironment.IsProduction())
            };


            containerBuilder.Register(_ => keys).As<IConfigurationKeys>();
            containerBuilder.RegisterAssemblyModules(assembliesOfProgram.ToArray());
            containerBuilder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly()).AsClosedTypesOf(typeof(IEventHandler<>));
            containerBuilder.RegisterType<Logger>().As<ILogger>();
            containerBuilder.RegisterType<DataProtection>().As<IDataProtect>();

            containerBuilder.RegisterType<SeoDocumentBuilder>()
                .Keyed<IBuildSeo>(SeoType.Document);
            containerBuilder.RegisterType<SeoTutorBuilder>()
                .Keyed<IBuildSeo>(SeoType.Tutor);
            containerBuilder.RegisterType<SeoStaticBuilder>()
                .Keyed<IBuildSeo>(SeoType.Static);
            containerBuilder.RegisterType<SeoQuestionBuilder>()
                .Keyed<IBuildSeo>(SeoType.Question);
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
        [UsedImplicitly]
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseHeaderRemover("X-HTML-Minification-Powered-By");
            app.UseClickJacking();

            if (env.IsDevelopment())
            {
                app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions
                {
                    HotModuleReplacement = true
                });
                app.UseDeveloperExceptionPage();

            }
            else
            {
                app.UseHsts();
            }
            app.UseStatusCodePagesWithReExecute("/Error/{0}");
            var reWriterOptions = new RewriteOptions()
                .Add(new RemoveTrailingSlash());
            if (!env.IsDevelopment() && !env.IsEnvironment(IntegrationTestEnvironmentName))
            {
                reWriterOptions.AddRedirectToHttpsPermanent();
                reWriterOptions.Add(new RedirectToWww());
            }

            app.UseRewriter(reWriterOptions);

            app.UseResponseCompression();
            app.UseResponseCaching();

            app.UseRequestLocalization(o =>
            {

                o.DefaultRequestCulture = new RequestCulture(Language.English);
                // Formatting numbers, dates, etc.
                o.SupportedUICultures = o.SupportedCultures = Language.SystemSupportLanguage().Select(s => (CultureInfo)s).ToList();// SupportedCultures;
                // UI strings that we have localized.
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
            //This is for ip
            //https://stackoverflow.com/a/41335701/1235448
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor |
                                   ForwardedHeaders.XForwardedProto
            });
            app.UseAuthentication();

            app.UseAzureSignalR(routes =>
            {
                routes.MapHub<SbHub>("/SbHub");
                routes.MapHub<StudyRoomHub>("/StudyRoomHub");
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: SeoTypeString.Question,
                    template: "question/{id:long}",
                    defaults: new { controller = "Home", action = "Index" }
                );

                //routes.MapRoute(
                //    name: SeoTypeString.Tutor,
                //    template: "profile/{id:long}/{name:string}",
                //    defaults: new { controller = "Home", action = "Index" }
                //);
                routes.MapRoute(
                    name: SeoTypeString.Static,
                    template: "{id}",
                    defaults: new { controller = "Home", action = "Index" }
                );
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action}/{id?}");
                routes.MapSpaFallbackRoute(
                    name: "spa-fallback",
                    defaults: new
                    {
                        controller = "Home",
                        action = "Index"
                    });
            });
        }
    }

    


}
