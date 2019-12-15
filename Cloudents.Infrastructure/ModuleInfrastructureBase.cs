using Autofac;
using Autofac.Extras.DynamicProxy;
using Cloudents.Core.Interfaces;
using Cloudents.Infrastructure.Interceptor;
using Cloudents.Infrastructure.Mail;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
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
            builder.RegisterAssemblyTypes(assembly)
                .AsClosedTypesOf(typeof(BaseTaskInterceptor<>));
            builder.RegisterType<RestClient>().As<IRestClient>()
                //.SingleInstance()
                .EnableInterfaceInterceptors()
                .InterceptedBy(typeof(LogInterceptor));


            builder.RegisterType<PayMePaymentProvider>().As<IPayment>();
            //builder.RegisterType<AzureTextAnalysisProvider>().As<ITextAnalysis>().SingleInstance();
            builder.RegisterType<BinarySerializer>().As<IBinarySerializer>();
            builder.RegisterType<SbJsonSerializer>().As<IJsonSerializer>();
            builder.RegisterType<MailProvider>().As<IMailProvider>();
            builder.RegisterType<CognitiveService>().As<ICognitiveService>().SingleInstance();
            builder.RegisterType<TwilioProvider>().AsSelf().As<ISmsProvider>().As<IVideoProvider>().SingleInstance();
            builder.RegisterType<CountryProvider>().As<ICountryProvider>();
        }
    }
}