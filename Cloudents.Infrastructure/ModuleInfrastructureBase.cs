﻿using Autofac;
using Autofac.Extras.DynamicProxy;
using Cloudents.Core.Interfaces;
using Cloudents.Infrastructure.Interceptor;
using Cloudents.Infrastructure.Mail;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Cloudents.Infrastructure.Payments;
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
                .EnableInterfaceInterceptors()
                .InterceptedBy(typeof(LogInterceptor));


            builder.RegisterType<PayMePaymentProvider>().As<IPayment>();
            builder.RegisterType<PayPalClient>().As<IPayPalService>().SingleInstance();
            builder.RegisterType<StripeClient>().As<IStripeService>().AsSelf();

            builder.RegisterType<BinarySerializer>().As<IBinarySerializer>();
            builder.RegisterType<SbJsonSerializer>().As<IJsonSerializer>();
            builder.RegisterType<MailProvider>().As<IMailProvider>();
            builder.RegisterType<CognitiveService>().As<ICognitiveService>().SingleInstance();
            builder.RegisterType<SmsProvider>().As<ISmsProvider>();
            builder.RegisterType<TwilioProvider>().AsSelf().As<IPhoneValidator>().As<IStudyRoomProvider>().SingleInstance();
            builder.RegisterType<CountryProvider>().As<ICountryProvider>();
            builder.RegisterType<WixBlogProvider>().As<IBlogProvider>()
                .EnableInterfaceInterceptors()
                .InterceptedBy(typeof(CacheResultInterceptor));
        }
    }
}