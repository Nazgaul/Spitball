using System.Reflection;
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
                .InterceptedBy(typeof(LogInterceptor), typeof(CacheResultInterceptor));
            builder.RegisterType<UniqueKeyGenerator>().As<IKeyGenerator>();
            builder.RegisterType<CacheResultInterceptor>();
            builder.RegisterType<LogInterceptor>();
            builder.RegisterType<RestClient>().As<IRestClient>().SingleInstance();

            //var config = MapperConfiguration();
            //builder.Register(c => config.CreateMapper()).SingleInstance();

            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly()).AsClosedTypesOf(typeof(ITypeConverter<,>));

            builder.Register(c => new MapperConfiguration(cfg =>
            {
                cfg.ConstructServicesUsing(c.Resolve);
                cfg.AddProfile<MapperProfile>();
            })).AsSelf().SingleInstance();

            //builder.Register(ctx => ctx.Resolve<MapperConfiguration>().CreateMapper()).As<AutoMapper.IMapper>();
            builder.Register(c => c.Resolve<MapperConfiguration>().CreateMapper(c.Resolve<IComponentContext>().Resolve))
                .As<IMapper>().InstancePerLifetimeScope();

            builder.RegisterType<Logger>().As<ILogger>();
        }
    }
}