using Autofac;
using Autofac.Extensions.DependencyInjection;
using Cloudents.Core;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Cloudents.Web.Binders;
using Cloudents.Web.Controllers;
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
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Cloudents.Web.Api;
using Microsoft.Extensions.DependencyModel;
using WebMarkupMin.AspNetCore2;
using Logger = Cloudents.Web.Services.Logger;


namespace Cloudents.Web
{
    public class Startup
    {

        public static IEnumerable<Assembly> GetAssemblies()
        {
            var list = new List<string>();
            var stack = new Stack<Assembly>();

            stack.Push(Assembly.GetEntryAssembly());

            do
            {
                var asm = stack.Pop();

                yield return asm;

                foreach (var reference in asm.GetReferencedAssemblies())
                    if (!list.Contains(reference.FullName))
                    {
                        stack.Push(Assembly.Load(reference));
                        list.Add(reference.FullName);
                    }

            }
            while (stack.Count > 0);

        }

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
                    o.Filters.Add<UserLockedExceptionFilter>();
                    o.Filters.Add(new GlobalExceptionFilter());
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

            services.AddDetectionCore().AddDevice().AddCrawler();
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
            //  services.AddSingleton<IHttpResponseStreamWriterFactory, SbMemoryPoolHttpResponseStreamWriterFactory>();
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
            services.AddTransient<ICountryProvider, CountryProvider>();


            var assembliesOfProgram = DependencyContext.Default.CompileLibraries
                .SelectMany(x => x.ResolveReferencePaths())
                .Distinct()
                .Where(x => x.Contains(Directory.GetCurrentDirectory()))
                .Select(Assembly.LoadFile)
                .ToList();
            
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
                ServiceBus = Configuration["ServiceBus"],
                PayPal = new PayPalCredentials(Configuration["PayPal:ClientId"], Configuration["PayPal:ClientSecret"], !HostingEnvironment.IsProduction())
            };

            containerBuilder.Register(_ => keys).As<IConfigurationKeys>();
            containerBuilder.RegisterAssemblyModules(assembliesOfProgram.ToArray());
            containerBuilder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly()).AsClosedTypesOf(typeof(IEventHandler<>));
            containerBuilder.RegisterType<Logger>().As<ILogger>();
            containerBuilder.RegisterType<DataProtection>().As<IDataProtect>();
            containerBuilder.RegisterType<UploadService>();

            //containerBuilder.RegisterType<DocumentSeoBuilder>()
            //    .As<IReadRepository<IEnumerable<SiteMapSeoDto>, SeoQuery>>()
            //    .WithParameter("query", SeoDbQuery.Flashcard);


            containerBuilder.RegisterType<DocumentSeoBuilder>()
                .Keyed<IBuildSeo>(SeoType.Document);
            containerBuilder.RegisterType<StaticSeoBuilder>()
                .Keyed<IBuildSeo>(SeoType.Static);
            containerBuilder.RegisterType<QuestionSeoBuilder>()
                .Keyed<IBuildSeo>(SeoType.Question);
            //
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
                app.UseHsts();
            }
            app.UseStatusCodePagesWithReExecute("/Error/{0}");
            var reWriterOptions = new RewriteOptions()
                .Add(new RemoveTrailingSlash());
            if (!env.IsDevelopment() && !env.IsEnvironment(IntegrationTestEnvironmentName))
            {
                reWriterOptions.AddRedirectToHttpsPermanent();
                //reWriterOptions.Add(new RedirectToWww());
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
            });


            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: SeoTypeString.Question,
                    template: "question/{id:long}",
                    defaults: new { controller = "Home", action = "Index" }
                );
                routes.MapRoute(
                    name: "Static",
                    template: "/{**page}",
                    defaults: new { controller = "Home", action = "Index" }
                );
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
