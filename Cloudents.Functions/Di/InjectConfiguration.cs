using System;
using System.Linq;
using System.Reflection;
using Autofac;
using Cloudents.Core;
using Cloudents.Core.Extension;
using Cloudents.Core.Interfaces;
using Cloudents.Functions.Sync;
using Cloudents.Infrastructure;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.WebJobs.Host.Config;

namespace Cloudents.Functions.Di
{
    public class InjectConfiguration : IExtensionConfigProvider
    {
        public void Initialize(ExtensionConfigContext context)
        {
            var services = new ContainerBuilder();
            RegisterServices(services);
            var container = services.Build();
            //var serviceProvider = services.BuildServiceProvider(true);

            context
                .AddBindingRule<InjectAttribute>()
                .Bind(new InjectBindingProvider(container));

            var registry = context.Config.GetService<IExtensionRegistry>();
            var filter = new ScopeCleanupFilter();
            registry.RegisterExtension(typeof(IFunctionInvocationFilter), filter);
            registry.RegisterExtension(typeof(IFunctionExceptionFilter), filter);
        }
        private static void RegisterServices(ContainerBuilder builder)
        {
            var keys = new ConfigurationKeys(
                GetEnvironmentVariable("SiteEndPoint") ?? "https://www.spitball.co")
            {
                Db = new DbConnectionString(GetEnvironmentVariable("ConnectionString"), GetEnvironmentVariable("Redis")),
                Redis = GetEnvironmentVariable("Redis"),
                Search = new SearchServiceCredentials(
                    GetEnvironmentVariable("SearchServiceName"),
                    GetEnvironmentVariable("SearchServiceAdminApiKey"),
                    bool.Parse(GetEnvironmentVariable("IsDevelop"))
                    ),
                MailGunDb = GetEnvironmentVariable("MailGunConnectionString"),
                BlockChainNetwork = GetEnvironmentVariable("BlockChainNetwork"),
                Storage = GetEnvironmentVariable("AzureWebJobsStorage")
            };

            builder.Register(_ => keys).As<IConfigurationKeys>();

            builder.RegisterSystemModules(
                Core.Enum.System.Function,
                Assembly.Load("Cloudents.Infrastructure.Framework"),
                Assembly.Load("Cloudents.Infrastructure"),
                Assembly.Load("Cloudents.Core"));

            builder.RegisterType<RestClient>().As<IRestClient>()
                .SingleInstance();

            builder.RegisterType<QuestionDbToSearchSync>().Keyed<IDbToSearchSync>(SyncType.Question);
            builder.RegisterType<UniversityDbToSearchSync>().Keyed<IDbToSearchSync>(SyncType.University);
            builder.RegisterType<CourseDbToSearchSync>().Keyed<IDbToSearchSync>(SyncType.Course);

            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        }

        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => a.FullName == args.Name).FirstOrDefault();
        }

        public static string GetEnvironmentVariable(string name)
        {
            return Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);
        }
    }
}
