using Autofac;
using Cloudents.Core.Entities.Search;
using Cloudents.Core.Interfaces;
using Cloudents.Infrastructure.Write;

namespace Cloudents.Infrastructure
{
    public class ModuleWrite : Module
    {
        private readonly SearchServiceCredentials _searchCredentials;
        private readonly string _redisConnection;

        public ModuleWrite(SearchServiceCredentials searchCredentials, string redisConnection)
        {
            _searchCredentials = searchCredentials;
            _redisConnection = redisConnection;
        }
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterModule(new ModuleInfrastructureBase(_searchCredentials, _redisConnection));
            builder.RegisterModule<ModuleMail>();

            builder.RegisterGeneric(typeof(SearchServiceWrite<>));
            builder.RegisterType<JobSearchWrite>().AsSelf().As<ISearchServiceWrite<Job>>().As<IStartable>().SingleInstance().AutoActivate();
        }
    }
}