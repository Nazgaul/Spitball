using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutoMapper;
using Cloudents.Core;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using Cloudents.Infrastructure;
using Cloudents.Web.Binders;
using Cloudents.Web.Extensions;
using Cloudents.Web.Filters;
using Cloudents.Web.Identity;
using Cloudents.Web.Middleware;
using Cloudents.Web.Swagger;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Converters;
using Swashbuckle.AspNetCore.Swagger;
using WebMarkupMin.AspNetCore2;

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
            services.AddWebMarkupMin().AddHtmlMinification();
            //services.AddLocalization(options => options.ResourcesPath = "Resources");
            services.AddMvc()
                //.AddRazorPagesOptions(options => { options.RootDirectory = "/Views"; })
                .AddJsonOptions(options =>
            {
                options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
                options.SerializerSettings.Converters.Add(new StringEnumConverter { CamelCaseText = true });
                options.SerializerSettings.Converters.Add(new IsoDateTimeConverter
                {
                    DateTimeStyles = DateTimeStyles.AssumeUniversal
                });
            }).AddMvcOptions(o =>
                {
                    o.Filters.Add(new GlobalExceptionFilter(HostingEnvironment));
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

            services.AddScoped<IPasswordHasher<User>, PasswordHasher>();
            services.AddIdentity<User, ApplicationRole>(options =>
                {
                    options.SignIn.RequireConfirmedEmail = true;
                    options.User.RequireUniqueEmail = true;
                    options.SignIn.RequireConfirmedPhoneNumber = true;
                    options.Password.RequiredLength = 1;
                    options.Password.RequireDigit = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequiredUniqueChars = 0;
                }).AddDefaultTokenProviders();

            services.AddAuthorization(options =>
            {
                options.AddPolicy(SignInStep.PolicyEmail,
                    policy => policy.RequireClaim(SignInStep.Claim, SignInStepEnum.Email.ToString("D")));
                options.AddPolicy(SignInStep.PolicySms,
                    policy => policy.RequireClaim(SignInStep.Claim, SignInStepEnum.Sms.ToString("D")));
                options.AddPolicy(SignInStep.PolicyPassword,
                    policy => policy.RequireClaim(SignInStep.Claim, SignInStepEnum.UntilPassword.ToString("D")));
                options.AddPolicy(SignInStep.PolicyAll,
                    policy => policy.RequireClaim(SignInStep.Claim, SignInStepEnum.All.ToString("D")));
            });
            services.ConfigureApplicationCookie(o =>
            {
                o.Cookie.Name = "sb1";
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
                //o.Events.OnValidatePrincipal = context =>
                //{
                //    return Task.CompletedTask;
                //};
            });
            services.AddAuthentication();

            services.AddScoped<IUserClaimsPrincipalFactory<User>, AppClaimsPrincipalFactory>();
            services.AddTransient<IUserStore<User>, UserStore>();
            services.AddTransient<IRoleStore<ApplicationRole>, RoleStore>();

            var assembliesOfProgram = new[]
            {
                Assembly.Load("Cloudents.Infrastructure.Framework"),
                Assembly.Load("Cloudents.Infrastructure.Storage"),
                Assembly.Load("Cloudents.Infrastructure"),
                Assembly.Load("Cloudents.Core"),
                Assembly.GetExecutingAssembly()
            };
            services.AddAutoMapper(assembliesOfProgram);

            var containerBuilder = new ContainerBuilder();
            services.AddSingleton<WebPackChunkName>();

            var keys = new ConfigurationKeys
            {
                Db = Configuration.GetConnectionString("DefaultConnection"),
                Search = new SearchServiceCredentials(Configuration["AzureSearch:SearchServiceName"],
                       Configuration["AzureSearch:SearchServiceAdminApiKey"]),
                Redis = Configuration["Redis"],
                Storage = Configuration["Storage"],
                FunctionEndpoint= Configuration["FunctionEndpoint"],
                BlockChainNetwork = Configuration["BlockChainNetwork"]
            };

            containerBuilder.Register(_ => keys).As<IConfigurationKeys>();

            containerBuilder.RegisterSystemModules(
                Core.Enum.System.Web, assembliesOfProgram);

            containerBuilder.Populate(services);
            var container = containerBuilder.Build();
            return new AutofacServiceProvider(container);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        [UsedImplicitly]
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseHeaderRemover("X-HTML-Minification-Powered-By");
            //var supportedCultures = new[]
            //{
            //    new CultureInfo("en-US")
            //};
            //app.UseRequestLocalization(new RequestLocalizationOptions
            //{
            //    DefaultRequestCulture = new RequestCulture(supportedCultures[0]),
            //    SupportedCultures = supportedCultures,
            //    SupportedUICultures = supportedCultures
            //});
            if (env.IsDevelopment())
            {
                app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions
                {
                    HotModuleReplacement = true
                });
            }
            if (env.IsDevelopment() || env.IsEnvironment(IntegrationTestEnvironmentName))
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            var reWriterOptions = new RewriteOptions()
                .Add(new RemoveTrailingSlash());
            if (!env.IsEnvironment(IntegrationTestEnvironmentName))
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

                routes.MapSpaFallbackRoute(
                    name: "spa-fallback",
                    defaults: new { controller = "Home", action = "Index" });
            });
        }
    }
}
