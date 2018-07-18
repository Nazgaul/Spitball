using System.Reflection;
using Autofac;
using Cloudents.Core.Attributes;
using Cloudents.Core.Interfaces;
using Cloudents.Infrastructure.Write;
using JetBrains.Annotations;
using Microsoft.Azure.Search;
using Module = Autofac.Module;

namespace Cloudents.Infrastructure
{
    [ModuleRegistration(Core.Enum.System.Console)]
    [ModuleRegistration(Core.Enum.System.WorkerRole)]
    [ModuleRegistration(Core.Enum.System.Function)]
    [UsedImplicitly]
    public class ModuleAzureSearch : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterGeneric(typeof(SearchServiceWrite<>));
            var assembly = Assembly.GetExecutingAssembly();
            builder.RegisterAssemblyTypes(assembly).AsClosedTypesOf(typeof(ISearchServiceWrite<>)).AsImplementedInterfaces();
            builder.Register(c =>
                {
                    var key = c.Resolve<IConfigurationKeys>().Search;
                    return new SearchServiceClient(key.Name, new SearchCredentials(key.Key));
                })
                .SingleInstance().AsSelf().As<ISearchServiceClient>();
        }
    }
}