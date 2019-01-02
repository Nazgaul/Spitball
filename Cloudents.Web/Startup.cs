using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutoMapper;
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
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Cloudents.Core;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Request;
using Cloudents.Infrastructure.Data;
using Cloudents.Search;
using Cloudents.Web.Resources;
using Microsoft.AspNetCore.HttpOverrides;
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

            services.AddDetectionCore().AddDevice();
            services.AddScoped<SignInManager<RegularUser>, SbSignInManager>();
            services.AddIdentity<RegularUser, ApplicationRole>(options =>
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


            services.AddScoped<IUserClaimsPrincipalFactory<RegularUser>, AppClaimsPrincipalFactory>();
            services.AddTransient<IUserStore<RegularUser>, UserStore>();
            services.AddTransient<IRoleStore<ApplicationRole>, RoleStore>();
            services.AddTransient<ISmsSender, SmsSender>();
            services.AddTransient<IProfileUpdater, QueueProfileUpdater>();
            services.AddTransient<ICountryProvider, CountryProvider>();
            //services.AddScoped<RedirectToOldSiteFilterAttribute>();
            var assembliesOfProgram = new[]
            {
                Assembly.Load("Cloudents.Infrastructure.Storage"),
                Assembly.Load("Cloudents.Infrastructure"),
                Assembly.Load("Cloudents.Core"),
                Assembly.Load("Cloudents.Persistance"),
                Assembly.Load("Cloudents.Query"),
                Assembly.GetExecutingAssembly()
            };

            /*var assembliesOfProgram = Assembly
            .GetExecutingAssembly()
            .GetReferencedAssemblies()
            .Select(Assembly.Load)
            .Where(t => t.FullName.Split('.')[0] == "Cloudents");*/


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
            containerBuilder.RegisterAssemblyModules(assembliesOfProgram);
            //containerBuilder.RegisterSystemModules(
            //    Application.Enum.System.Web, assembliesOfProgram);
            containerBuilder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly()).AsClosedTypesOf(typeof(IEventHandler<>));
            containerBuilder.RegisterType<Logger>().As<ILogger>();
            containerBuilder.RegisterType<DataProtection>().As<IDataProtect>();

            containerBuilder.RegisterModule(new SearchModule(Configuration["AzureSearch:SearchServiceName"],
                Configuration["AzureSearch:SearchServiceAdminApiKey"], !HostingEnvironment.IsProduction()));

            containerBuilder.RegisterType<SeoDocumentRepository>()
                .As<IReadRepository<IEnumerable<SiteMapSeoDto>, SeoQuery>>().WithParameter("query", SeoDbQuery.Flashcard);

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
                app.UseDeveloperExceptionPage();

            }
            else
            {
                //app.UseStatusCodePagesWithReExecute("/Error/{0}");
                //app.UseExceptionHandler("/Error");
                app.UseHsts();
                //app.UseHsts(new HstsOptions()
                //{
                //    IncludeSubDomains = true,
                //    Preload = true
                //});
            }
            app.UseStatusCodePagesWithReExecute("/Error/{0}");
            var reWriterOptions = new RewriteOptions()
                .Add(new RemoveTrailingSlash());
            if (!env.IsDevelopment() && !env.IsEnvironment(IntegrationTestEnvironmentName))
            {
                reWriterOptions.AddRedirectToHttpsPermanent();
            }

            app.UseRewriter(reWriterOptions);

            app.UseResponseCompression();
            app.UseResponseCaching();

            //app.UseStatusCodePages();
            

            app.UseRequestLocalization(o =>
            {

                o.DefaultRequestCulture = new RequestCulture(Language.English);
                // Formatting numbers, dates, etc.
                o.SupportedCultures = Language.SystemSupportLanguage;// SupportedCultures;
                // UI strings that we have localized.
                o.SupportedUICultures = Language.SystemSupportLanguage;
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
            
            app.UseAzureSignalR(routes =>
            {
                routes.MapHub<SbHub>("/SbHub");
            });
            //This is for ip
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor |
                                   ForwardedHeaders.XForwardedProto
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
    }
}
