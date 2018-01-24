using System.IO;
using Autofac;
using Cloudents.Core.Entities.Search;
using Cloudents.Core.Interfaces;
using Cloudents.Infrastructure.Storage;
using Cloudents.Infrastructure.Write;

namespace Cloudents.Infrastructure
{
    public class ModuleWrite : Module
    {
        private readonly SearchServiceCredentials _searchCredentials;
        private readonly string _redisConnection;
        private readonly LocalStorageData _localStorageData;

        public ModuleWrite(SearchServiceCredentials searchCredentials, string redisConnection, LocalStorageData localStorageData)
        {
            _searchCredentials = searchCredentials;
            _redisConnection = redisConnection;
            _localStorageData = localStorageData;
        }
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterModule(new ModuleInfrastructureBase(_searchCredentials, _redisConnection));
            builder.RegisterModule<ModuleMail>();

            builder.RegisterGeneric(typeof(SearchServiceWrite<>));
            builder.RegisterType<JobSearchWrite>().AsSelf().As<ISearchServiceWrite<Job>>().As<IStartable>().SingleInstance().AutoActivate();
            builder.RegisterType<DownloadFile>().As<IDownloadFile>();
            builder.Register(c =>
                new TempStorageProvider(c.Resolve<ILogger>(), _localStorageData)).As<ITempStorageProvider>();
        }
    }
}