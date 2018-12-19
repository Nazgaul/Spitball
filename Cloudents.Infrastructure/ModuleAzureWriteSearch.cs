using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Autofac;
using Cloudents.Application.Attributes;
using Cloudents.Application.Interfaces;
using Cloudents.Infrastructure.Search;
using Cloudents.Infrastructure.Write;
using Module = Autofac.Module;

namespace Cloudents.Infrastructure
{
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Ioc Module registration by reflection")]
    [ModuleRegistration(Application.Enum.System.Console)]
    [ModuleRegistration(Application.Enum.System.WorkerRole)]
    [ModuleRegistration(Application.Enum.System.Function)]
    public class ModuleAzureWriteSearch : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterGeneric(typeof(SearchServiceWrite<>));
            var assembly = Assembly.GetExecutingAssembly();
            builder.RegisterAssemblyTypes(assembly).AsClosedTypesOf(typeof(ISearchServiceWrite<>))
                .AsImplementedInterfaces();


            builder.RegisterType<SearchService>().AsSelf().As<ISearchService>().SingleInstance();
            builder.RegisterType<TextAnalysisProvider>().As<ITextAnalysis>();
        }
    }

    [ModuleRegistration(Application.Enum.System.Function)]
    [ModuleRegistration(Application.Enum.System.Web)]
    [ModuleRegistration(Application.Enum.System.Console)]
    public class ModuleInfrastructure : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<TextAnalysisProvider>().As<ITextAnalysis>();
        }
    }
}