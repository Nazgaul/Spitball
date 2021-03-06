﻿using Autofac;
using Cloudents.Core.Interfaces;
using Cloudents.Infrastructure.Mail;
using System.Diagnostics.CodeAnalysis;
using Cloudents.Core.Entities;
using Cloudents.Infrastructure.Image;
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
            builder.RegisterType<RestClient>().As<IRestClient>();

            builder.RegisterType<PayMePaymentProvider>().As<IPaymeProvider>().Keyed<IPaymentProvider>(typeof(PaymePayment));
            builder.RegisterType<StripeClient>().As<IStripeService>().Keyed<IPaymentProvider>(typeof(StripePayment)).AsSelf();

            builder.RegisterType<BinarySerializer>().As<IBinarySerializer>();
            builder.RegisterType<SbJsonSerializer>().As<IJsonSerializer>();
            builder.RegisterType<MailProvider>().As<IMailProvider>();
            builder.RegisterType<CognitiveService>().As<ICognitiveService>().SingleInstance();
            builder.RegisterType<SmsProvider>().As<ISmsProvider>();
            builder.RegisterType<TwilioProvider>().AsSelf().As<IPhoneValidator>().As<IStudyRoomProvider>().SingleInstance();
            builder.RegisterType<CountryProvider>().As<ICountryProvider>();
            builder.RegisterType<WixBlogProvider>().As<IBlogProvider>();
           // builder.RegisterType<CronService>().As<ICronService>();
            builder.RegisterType<IpStackProvider>().As<IIpToLocation>();
            builder.RegisterType<ImageProcessor>().As<IImageProcessor>();
        }
    }
}