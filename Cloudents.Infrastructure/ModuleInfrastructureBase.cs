using Autofac;
using Autofac.Extras.DynamicProxy;
using AutoMapper;
using Cloudents.Core.Interfaces;
using Cloudents.Infrastructure.Cache;
using Cloudents.Infrastructure.Search.Places;
using Microsoft.Azure.Search;
using Newtonsoft.Json.Serialization;

namespace Cloudents.Infrastructure
{
    public class ModuleInfrastructureBase : Module
    {
        private readonly SearchServiceCredentials _searchService;
        private readonly string _redisConnectionString;

        public ModuleInfrastructureBase(SearchServiceCredentials searchService, string redisConnectionString)
        {
            _searchService = searchService;
            _redisConnectionString = redisConnectionString;
        }


        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterModule(new ModuleCache(_redisConnectionString));
            builder.Register(c => new SearchServiceClient(_searchService.Name, new SearchCredentials(_searchService.Key)))
                .SingleInstance().AsSelf().As<ISearchServiceClient>();

            builder.RegisterType<GooglePlacesSearch>().As<IGooglePlacesSearch>().EnableInterfaceInterceptors()
                .InterceptedBy(typeof(CacheResultInterceptor));
            builder.RegisterType<UniqueKeyGenerator>().As<IKeyGenerator>();
            builder.RegisterType<CacheResultInterceptor>();
            builder.RegisterType<RestClient>().As<IRestClient>();

            var config = MapperConfiguration();
            builder.Register(c => config.CreateMapper()).SingleInstance();
            builder.RegisterType<CacheProvider>().AsImplementedInterfaces();

        }
        private static MapperConfiguration MapperConfiguration()
        {
            return new MapperConfiguration(cfg => cfg.AddProfile<MapperProfile>());
        }
    }
}