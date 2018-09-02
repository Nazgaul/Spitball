﻿using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Autofac;
using Cloudents.Core.Attributes;
using Cloudents.Core.Interfaces;
using Cloudents.Infrastructure.Search;
using Cloudents.Infrastructure.Write;
using Module = Autofac.Module;

namespace Cloudents.Infrastructure
{
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Ioc Module registration by reflection")]
    [ModuleRegistration(Core.Enum.System.Console)]
    [ModuleRegistration(Core.Enum.System.WorkerRole)]
    [ModuleRegistration(Core.Enum.System.Function)]
    public class ModuleAzureWriteSearch : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterGeneric(typeof(SearchServiceWrite<>));
            var assembly = Assembly.GetExecutingAssembly();
            builder.RegisterAssemblyTypes(assembly).AsClosedTypesOf(typeof(ISearchServiceWrite<>)).AsImplementedInterfaces();

            builder.RegisterType<SearchService>().AsSelf().As<ISearchService>().SingleInstance();

            //builder.Register(c =>
            //    {
            //        var key = c.Resolve<IConfigurationKeys>().Search;
            //        return new SearchServiceClient(key.Name, new SearchCredentials(key.Key));
            //    })
            //    .SingleInstance().AsSelf().As<ISearchServiceClient>();

            //builder.RegisterType<SearchService>().As<ISearchService>().SingleInstance();
        }
    }
}