using System;
using System.Linq;
using System.Reflection;
using Autofac;
using Cloudents.Infrastructure.Framework;
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
            //var keys = new ConfigurationKeys(
            //    GetEnvironmentVariable("SiteEndPoint") ?? "https://www.spitball.co")
            //{
            //    Db = new DbConnectionString(GetEnvironmentVariable("ConnectionString"), GetEnvironmentVariable("Redis")),
            //    Redis = GetEnvironmentVariable("Redis"),
            //    Search = new SearchServiceCredentials(
            //        GetEnvironmentVariable("SearchServiceName"),
            //        GetEnvironmentVariable("SearchServiceAdminApiKey"),
            //        bool.Parse(GetEnvironmentVariable("IsDevelop"))
            //        ),
            //    MailGunDb = GetEnvironmentVariable("MailGunConnectionString"),
            //    BlockChainNetwork = GetEnvironmentVariable("BlockChainNetwork"),
            //    Storage = GetEnvironmentVariable("AzureWebJobsStorage")
            //};

            //builder.Register(_ => keys).As<IConfigurationKeys>();

            builder.RegisterModule<ModuleFile>();
            //builder.RegisterSystemModules(
            //    Core.Enum.System.Function,
            //    Assembly.Load("Cloudents.Infrastructure.Framework"));





            //AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        }

        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            return AppDomain.CurrentDomain
                .GetAssemblies().FirstOrDefault(a => a.FullName == args.Name);
        }

        public static string GetEnvironmentVariable(string name)
        {
            return Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);
        }
    }
}
