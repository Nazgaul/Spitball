using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Cloudents.Core;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Cloudents.Infrastructure.Video;
using Cloudents.Web.Binders;
using Cloudents.Web.Filters;
using Cloudents.Web.Hubs;
using Cloudents.Web.Identity;
using Cloudents.Web.Middleware;
using Cloudents.Web.Resources;
using Cloudents.Web.Services;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Newtonsoft.Json;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Cloudents.Web.Seo;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Newtonsoft.Json.Serialization;
using WebMarkupMin.AspNetCore3;
using WebMarkupMin.Core;
using Logger = Cloudents.Web.Services.Logger;


namespace Cloudents.Web
{
    public class Startup
    {
        public const string IntegrationTestEnvironmentName = "Integration-Test";
        //private const bool UseAzureSignalR = true;

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            HostingEnvironment = env;
        }

        private IConfiguration Configuration { get; }
        private IWebHostEnvironment HostingEnvironment { get; }

        public ILifetimeScope AutofacContainer { get; private set; }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to conapp\Cloudents.Web\Startup.csfigure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940

        public void ConfigureContainer(ContainerBuilder containerBuilder)
        {
            // Register your own things directly with Autofac, like:
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



            // var containerBuilder = new ContainerBuilder();
            containerBuilder.Register(c =>
            {
                var val = c.Resolve<IOptionsMonitor<PayMeCredentials>>();
                return val.CurrentValue;
            }).AsSelf();

            var dbActionStr = Configuration["DbAction"];
            
            if (!Enum.TryParse(dbActionStr, true, out DbConnectionString.DataBaseIntegration dbAction))
            {
                dbAction = DbConnectionString.DataBaseIntegration.Validate;
            }

            var keys = new ConfigurationKeys()
            {
                SiteEndPoint = { SpitballSite = Configuration["Site"], FunctionSite = Configuration["functionCdnEndpoint"] },
                Db = new DbConnectionString(Configuration.GetConnectionString("DefaultConnection"),
                    Configuration["Redis"], dbAction)
               ,
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


            containerBuilder.RegisterType<MediaServices>().SingleInstance().As<IVideoService>().WithParameter("isDevelop", keys.Search.IsDevelop);


            containerBuilder.RegisterType<SeoDocumentBuilder>()
                .Keyed<IBuildSeo>(SeoType.Document);
            containerBuilder.RegisterType<SeoTutorBuilder>()
                .Keyed<IBuildSeo>(SeoType.Tutor);
            containerBuilder.RegisterType<SeoStaticBuilder>()
                .Keyed<IBuildSeo>(SeoType.Static);
            containerBuilder.RegisterType<SeoQuestionBuilder>()
                .Keyed<IBuildSeo>(SeoType.Question);

            containerBuilder.RegisterType<SeoCourseTutorBuilder>()
                .Keyed<IBuildSeo>(SeoType.TutorList);

            containerBuilder.RegisterType<VideoServiceGenerator>()
                .Keyed<IDocumentGenerator>(DocumentType.Video);
            containerBuilder.RegisterType<DocumentPreviewGenerator>()
                .Keyed<IDocumentGenerator>(DocumentType.Document);

            containerBuilder.Register(c =>
            {
                var z = c.Resolve<IHttpClientFactory>();
                return z.CreateClient();
            });
            //containerBuilder.Populate(services);
            //var container = containerBuilder.Build();
            //return new AutofacServiceProvider(container);
        }

        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddSingleton<ITelemetryInitializer, RequestBodyInitializer>();
            services.AddSingleton<ITelemetryInitializer, UserIdInitializer>();
            services.AddApplicationInsightsTelemetry();

            services.Configure<ForwardedHeadersOptions>(options =>
            {

                options.ForwardedHeaders =
                    ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
                                                   | ForwardedHeaders.XForwardedHost;
                options.KnownNetworks.Clear(); //TODO: need to do
                options.KnownProxies.Clear();//TODO: need to do

            });
            services.AddLocalization(x => x.ResourcesPath = "Resources");
            services.AddDataProtection(o =>
            {
                o.ApplicationDiscriminator = "spitball";
            }).PersistKeysToAzureBlobStorage(CloudStorageAccount.Parse(Configuration["Storage"]), "/spitball/keys/keys.xml");

            services.AddWebMarkupMin().AddHtmlMinification(x =>
                {
                    x.MinificationSettings.AttributeQuotesRemovalMode = HtmlAttributeQuotesRemovalMode.KeepQuotes;
                });
            services.AddControllersWithViews();
            //services.AddRouting(x =>
            //{
            //    // x.ConstraintMap.Add("StorageContainerConstraint", typeof(StorageContainerRouteConstraint));
            //});

            services.AddControllersWithViews()

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
                //ToDO use the new one
                .AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                options.SerializerSettings.Converters.Add(new StringEnumNullUnknownStringConverter { NamingStrategy = new CamelCaseNamingStrategy() });
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
                .ConfigureApiBehaviorOptions(o =>
                {
                    o.SuppressMapClientErrors = true; //https://github.com/aspnet/AspNetCore/issues/4792#issuecomment-454164457
                                                      // o.SuppressUseValidationProblemDetailsForInvalidModelStateResponses = false;
                    o.InvalidModelStateResponseFactory = actionContext =>
                    {
                        var telemetryClient = actionContext.HttpContext.RequestServices.GetService<TelemetryClient>();


                        var errorList = actionContext.ModelState.ToDictionary(
                            kvp => kvp.Key,
                            kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).FirstOrDefault()
                        );
                        telemetryClient.TrackEvent($"Bad request - {actionContext.HttpContext.Request.GetDisplayUrl()}", errorList);
                        return new BadRequestObjectResult(actionContext.ModelState);
                    };
                });
            if (HostingEnvironment.IsDevelopment() || HostingEnvironment.IsStaging())
            {
                Swagger.Startup.SwaggerInitial(services);
            }

            var t = services.AddSignalR().AddNewtonsoftJsonProtocol(o =>
            {
                o.PayloadSerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                o.PayloadSerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                o.PayloadSerializerSettings.Converters.Add(new StringEnumNullUnknownStringConverter
                { NamingStrategy = new CamelCaseNamingStrategy() });
            });
            //if (UseAzureSignalR)
            //{
            t.AddAzureSignalR();
            //}
            services.AddResponseCompression();
            services.AddResponseCaching();
            var physicalProvider = HostingEnvironment.ContentRootFileProvider;
            services.AddSingleton(physicalProvider);

            services.AddDetectionCore().AddCrawler();
            services.AddSbIdentity();

            services.AddResponseCompression(x =>
            {
                x.Providers.Add<BrotliCompressionProvider>();
                x.Providers.Add<GzipCompressionProvider>();
                x.EnableForHttps = true;
                x.MimeTypes = new[] {"text/javascript",
                    "application/javascript",
                    "text/css",
                    "text/html","application/json"};
            });

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
            //services.AddScoped<IRoleStore<UserRole>, RoleStore>();
            services.AddScoped<ISmsSender, SmsSender>();
            services.AddScoped<ICountryService, CountryService>();
            services.AddSingleton<ConfigurationService>();
            services.AddHttpClient();
            services.AddOptions();
            services.Configure<PayMeCredentials>(Configuration.GetSection("PayMe"));
            services.AddSingleton<WebPackChunkName>();



        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            AutofacContainer = app.ApplicationServices.GetAutofacRoot();
            app.UseHeaderRemover("X-HTML-Minification-Powered-By");
            app.UseForwardedHeaders();
            app.UseClickJacking();
            app.UseResponseCompression();
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
            }

            app.UseRewriter(reWriterOptions);


            app.UseResponseCaching();

           
            app.UseStaticFiles(new StaticFileOptions
            {
                OnPrepareResponse = ctx =>
                {
                    if (!env.IsDevelopment())
                    {
                        ctx.Context.Response.Headers.Add("Cache-Control", $"public,max-age={TimeConst.Year}");
                        ctx.Context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
                    }
                }
            });

            app.UseWebMarkupMin();

            if (env.IsDevelopment() || env.IsStaging())
            {
                app.UseSwagger();
                // Enable middleWare to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1"));
            }
            app.UseRouting();
            //This is for ip
            //https://stackoverflow.com/a/41335701/1235448
            //app.UseForwardedHeaders(new ForwardedHeadersOptions
            //{
            //    ForwardedHeaders = ForwardedHeaders.All
            //});
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseRequestLocalization(o =>
            {
                o.DefaultRequestCulture = new RequestCulture(Language.English);
                o.SupportedUICultures = o.SupportedCultures = Language.SystemSupportLanguage().Select(s => (CultureInfo)s).ToList();// SupportedCultures;

                o.RequestCultureProviders.Clear();
                o.RequestCultureProviders.Add(new FrymoCultureProvider());
                o.RequestCultureProviders.Add(new QueryStringRequestCultureProvider());
                o.RequestCultureProviders.Add(new FacebookQueryStringRequestCultureProvider());
                o.RequestCultureProviders.Add(new CookieRequestCultureProvider());
                o.RequestCultureProviders.Add(new AuthorizedUserCultureProvider());
                o.RequestCultureProviders.Add(new CountryCultureProvider());
                o.RequestCultureProviders.Add(new AcceptLanguageHeaderRequestCultureProvider());

            });

            //if (UseAzureSignalR)
            //{
            app.UseAzureSignalR(routes =>
            {
                routes.MapHub<SbHub>("/SbHub");
                routes.MapHub<StudyRoomHub>("/StudyRoomHub");
            });
            //}
            //else
            //{

            //    //app.UseSignalR(routes =>
            //    //{
            //    //    routes.MapHub<SbHub>("/SbHub");
            //    //    routes.MapHub<StudyRoomHub>("/StudyRoomHub");
            //    //});
            //}
            app.UseMiddleware<ApplicationInsightMiddleware>();
            app.UseEndpoints(endpoints =>
            {
                //routes.MapRoute(
                //    name: SeoTypeString.Question,
                //    template: "question/{id:long}",
                //    defaults: new { controller = "Home", action = "Index" }
                //);
                endpoints.MapControllerRoute(
                    name: SeoTypeString.Static,
                    pattern: "{id}",
                    defaults: new { controller = "Home", action = "Index" }
                );
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapFallbackToController("Index", "Home");

            });
            //app.UseMvc(routes =>
            //{
            //    routes.MapRoute(
            //        name: SeoTypeString.Question,
            //        template: "question/{id:long}",
            //        defaults: new { controller = "Home", action = "Index" }
            //    );

            //    //routes.MapRoute(
            //    //    name: SeoTypeString.TutorList,
            //    //    template: "tutor-list/{id}",
            //    //    defaults: new { controller = "Home", action = "Index" }
            //    //);
            //    routes.MapRoute(
            //        name: SeoTypeString.Static,
            //        template: "{id}",
            //        defaults: new { controller = "Home", action = "Index" }
            //    );
            //    routes.MapRoute(
            //        name: "default",
            //        template: "{controller}/{action}/{id?}",
            //        defaults: new { controller = "Home", action = "Index" });
            //    routes.MapSpaFallbackRoute(
            //        name: "spa-fallback",
            //        defaults: new
            //        {
            //            controller = "Home",
            //            action = "Index"
            //        });
            //});
        }
    }




}
