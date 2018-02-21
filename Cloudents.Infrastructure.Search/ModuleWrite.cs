using Autofac;
using Cloudents.Core.Entities.Search;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Microsoft.Azure.Search;

namespace Cloudents.Infrastructure.Search
{
    public class ModuleWrite : Module
    {
        //private readonly SearchServiceCredentials _searchCredentials;
        //private readonly string _redisConnection;
        //private readonly LocalStorageData _localStorageData;

        //public ModuleWrite(SearchServiceCredentials searchCredentials, string redisConnection, LocalStorageData localStorageData)
        //{
        //    _searchCredentials = searchCredentials;
        //    _redisConnection = redisConnection;
        //    _localStorageData = localStorageData;
        //}

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            //builder.RegisterModule<ModuleInfrastructureBase>();
            builder.Register(c =>
                {
                    var key = c.Resolve<IConfigurationKeys>().Search;
                    return new SearchServiceClient(key.Name, new SearchCredentials(key.Key));
                })
                .SingleInstance().AsSelf().As<ISearchServiceClient>();
            //builder.RegisterModule<ModuleMail>();

            builder.RegisterGeneric(typeof(SearchServiceWrite<>));
            //builder.RegisterType<JobSearchWrite>().AsSelf().As<ISearchServiceWrite<Job>>().As<IStartable>().SingleInstance().AutoActivate();
            //builder.RegisterType<TutorSearchWrite>().AsSelf().As<ISearchServiceWrite<Tutor>>().As<IStartable>().SingleInstance().AutoActivate();
            builder.RegisterType<UniversitySearchWrite>().AsSelf().As<ISearchServiceWrite<University>>().As<IStartable>().SingleInstance().AutoActivate();
            //builder.RegisterType<DownloadFile>().As<IDownloadFile>();
            //builder.Register(c =>
            //{
            //    var key = c.Resolve<IConfigurationKeys>().LocalStorageData;
            //    return new TempStorageProvider(c.Resolve<ILogger>(), key);
            //}).As<ITempStorageProvider>();

            //builder.RegisterType<JobCareerBuilder>().Keyed<IUpdateAffiliate>(AffiliateProgram.CareerBuilder);
            //builder.RegisterType<JobWayUp>().Keyed<IUpdateAffiliate>(AffiliateProgram.WayUp);
            //builder.RegisterType<TutorWyzant>().Keyed<IUpdateAffiliate>(AffiliateProgram.Wyzant);
        }
    }
}