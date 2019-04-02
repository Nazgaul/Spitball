using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Autofac;
using Autofac.Extras.DynamicProxy;
using Cloudents.Core.Interfaces;
using Cloudents.Infrastructure.Interceptor;
using Cloudents.Infrastructure.Mail;
using Cloudents.Infrastructure.Search.Places;
using Module = Autofac.Module;

namespace Cloudents.Infrastructure
{
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Ioc Module registration by reflection")]
    public class ModuleInfrastructureBase : Module
    {
       protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            var assembly = Assembly.GetExecutingAssembly();
           // builder.RegisterType<SearchService>().As<ISearchService>().SingleInstance();

            builder.RegisterType<GooglePlacesSearch>().As<IGooglePlacesSearch>().EnableInterfaceInterceptors()
                .InterceptedBy(typeof(CacheResultInterceptor));
            builder.RegisterType<UniqueKeyGenerator>().As<IKeyGenerator>();
            builder.RegisterAssemblyTypes(assembly)
                .AsClosedTypesOf(typeof(BaseTaskInterceptor<>));
            builder.RegisterType<RestClient>().As<IRestClient>()
                .SingleInstance().EnableInterfaceInterceptors()
                .InterceptedBy(typeof(LogInterceptor));


            builder.RegisterType<PayPalClient>().As<IPayPal>().SingleInstance();
            builder.RegisterType<AzureTextAnalysisProvider>().As<ITextAnalysis>().SingleInstance();
            builder.RegisterType<BinarySerializer>().As<IBinarySerializer>();
            builder.RegisterType<BinarySerializer>().As<IBinarySerializer>();
            builder.RegisterType<SmsProvider>().As<ISmsProvider>().As<IVideoProvider>().SingleInstance();

        }
    }
}