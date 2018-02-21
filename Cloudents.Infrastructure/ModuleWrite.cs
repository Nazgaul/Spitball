﻿using System.Reflection;
using Autofac;
using Cloudents.Core.Entities.Search;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Cloudents.Infrastructure.Data;
using Cloudents.Infrastructure.Storage;
using Cloudents.Infrastructure.Write;
using Cloudents.Infrastructure.Write.Job;
using Cloudents.Infrastructure.Write.Tutor;
using Microsoft.Azure.Search;
using Module = Autofac.Module;

namespace Cloudents.Infrastructure
{
    public class ModuleWrite : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterModule<ModuleInfrastructureBase>();
            //builder.RegisterModule<ModuleMail>();

            builder.RegisterGeneric(typeof(SearchServiceWrite<>));
            builder.RegisterType<JobSearchWrite>().AsSelf().As<ISearchServiceWrite<Job>>().As<IStartable>().SingleInstance().AutoActivate();
            builder.RegisterType<TutorSearchWrite>().AsSelf().As<ISearchServiceWrite<Tutor>>().As<IStartable>().SingleInstance().AutoActivate();
            builder.RegisterType<UniversitySearchWrite>().AsSelf().As<ISearchServiceWrite<University>>().As<IStartable>().SingleInstance().AutoActivate();
            //builder.RegisterType<SynonymWrite>().As<ISynonymWrite>();
            builder.RegisterType<DownloadFile>().As<IDownloadFile>();
            builder.Register(c =>
            {
                var key = c.Resolve<IConfigurationKeys>().LocalStorageData;
                return new TempStorageProvider(c.Resolve<ILogger>(), key);
            }).As<ITempStorageProvider>();

            builder.RegisterType<JobCareerBuilder>().Keyed<IUpdateAffiliate>(AffiliateProgram.CareerBuilder);
            builder.RegisterType<JobWayUp>().Keyed<IUpdateAffiliate>(AffiliateProgram.WayUp);
            builder.RegisterType<TutorWyzant>().Keyed<IUpdateAffiliate>(AffiliateProgram.Wyzant);
        }
    }

    public class ModuleAzureSearch : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
           // builder.RegisterType<SynonymWrite>().As<ISynonymWrite>();
            builder.RegisterGeneric(typeof(SearchServiceWrite<>));
            builder.RegisterType<UniversitySearchWrite>().AsSelf().As<ISearchServiceWrite<University>>().As<IStartable>().SingleInstance().AutoActivate();
            builder.Register(c =>
                {
                    var key = c.Resolve<IConfigurationKeys>().Search;
                    return new SearchServiceClient(key.Name, new SearchCredentials(key.Key));
                })
                .SingleInstance().AsSelf().As<ISearchServiceClient>();
        }
    }

    public class ModuleReadDb : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var currentAssembly = Assembly.GetExecutingAssembly();
            builder.Register(c =>
            {
                var key = c.Resolve<IConfigurationKeys>().Db;
                return new DapperRepository(key);
            });
            builder.RegisterAssemblyTypes(currentAssembly).AsClosedTypesOf(typeof(IReadRepositoryAsync<,>));
            builder.RegisterAssemblyTypes(currentAssembly).AsClosedTypesOf(typeof(IReadRepositoryAsync<>));

        }
    }
}