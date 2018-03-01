﻿using System.Reflection;
using Autofac;
using Autofac.Extras.DynamicProxy;
using AutoMapper;
using Cloudents.Core;
using Cloudents.Core.Interfaces;
using Cloudents.Infrastructure.Interceptor;
using Cloudents.Infrastructure.Search.Places;
using Microsoft.Azure.Search;
using Module = Autofac.Module;

namespace Cloudents.Infrastructure
{
    public class ModuleInfrastructureBase : Module
    {
       protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            var assembly = Assembly.GetExecutingAssembly();
            builder.RegisterModule<ModuleCache>();
            builder.Register(c =>
                {
                    var key = c.Resolve<IConfigurationKeys>().Search;
                    return new SearchServiceClient(key.Name, new SearchCredentials(key.Key));
                })
                .SingleInstance().AsSelf().As<ISearchServiceClient>();

            builder.RegisterType<GooglePlacesSearch>().As<IGooglePlacesSearch>().EnableInterfaceInterceptors()
                .InterceptedBy(typeof(CacheResultInterceptor));
            builder.RegisterType<UniqueKeyGenerator>().As<IKeyGenerator>();
            builder.RegisterAssemblyTypes(assembly)
                .AsClosedTypesOf(typeof(BaseTaskInterceptor<>));
            builder.RegisterType<RestClient>().As<IRestClient>()
                .SingleInstance().EnableInterfaceInterceptors()
                .InterceptedBy(typeof(LogInterceptor));

            builder.RegisterAssemblyTypes(assembly)
                .AsClosedTypesOf(typeof(ITypeConverter<,>));

            builder.Register(c => new MapperConfiguration(cfg =>
            {
                cfg.ConstructServicesUsing(c.Resolve);
                cfg.AddProfiles(assembly);
            })).AsSelf().SingleInstance();

            builder.Register(c => c.Resolve<MapperConfiguration>().CreateMapper(c.Resolve<IComponentContext>().Resolve))
                .As<IMapper>().InstancePerLifetimeScope();

            builder.RegisterType<Logger>().As<ILogger>();
        }
    }
}