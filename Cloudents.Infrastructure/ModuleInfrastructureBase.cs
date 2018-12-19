using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Autofac;
using Autofac.Extras.DynamicProxy;
using AutoMapper;
using Cloudents.Application.Attributes;
using Cloudents.Application.Interfaces;
using Cloudents.Infrastructure.Interceptor;
using Cloudents.Infrastructure.Search;
using Cloudents.Infrastructure.Search.Places;
using Module = Autofac.Module;

namespace Cloudents.Infrastructure
{
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Ioc Module registration by reflection")]
    [ModuleRegistration(Application.Enum.System.Console)]
    [ModuleRegistration(Application.Enum.System.Web)]
    [ModuleRegistration(Application.Enum.System.WorkerRole)]
    [ModuleRegistration(Application.Enum.System.Admin)]
    public class ModuleInfrastructureBase : Module
    {
       protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            var assembly = Assembly.GetExecutingAssembly();
            builder.RegisterType<SearchService>().As<ISearchService>().SingleInstance();

            builder.RegisterType<GooglePlacesSearch>().As<IGooglePlacesSearch>().EnableInterfaceInterceptors()
                .InterceptedBy(typeof(CacheResultInterceptor));
            builder.RegisterType<UniqueKeyGenerator>().As<IKeyGenerator>();
            builder.RegisterAssemblyTypes(assembly)
                .AsClosedTypesOf(typeof(BaseTaskInterceptor<>));
            builder.RegisterType<RestClient>().As<IRestClient>()
                .SingleInstance().EnableInterfaceInterceptors()
                .InterceptedBy(typeof(LogInterceptor));


            builder.RegisterType<TextAnalysisProvider>().As<ITextAnalysis>().SingleInstance();

            builder.RegisterAssemblyTypes(assembly)
                .AsClosedTypesOf(typeof(ITypeConverter<,>));

        }
    }
}