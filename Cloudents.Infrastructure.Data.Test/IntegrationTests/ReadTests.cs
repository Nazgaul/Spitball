using Autofac;
using Cloudents.Core;
using Cloudents.Core.Interfaces;
using Cloudents.Query;
using Cloudents.Query.Documents;
using System.Threading.Tasks;
using Xunit;

namespace Cloudents.Infrastructure.Data.Test.IntegrationTests
{
    public class ReadTests
    {
        //private readonly DapperRepository _dapperRepository;
        //private readonly AutoMock _autoMock;
        private readonly IQueryBus _queryBus;
        // private readonly IContainer _container;

        public ReadTests()
        {
            var configuration = new ConfigurationKeys("SomeSite")
            {
                Db = new DbConnectionString(
                    "Server=tcp:on0rodxe8f.database.windows.net;Database=ZBoxNew_Develop;User ID=ZBoxAdmin@on0rodxe8f;Password=Pa$$W0rd;Trusted_Connection=False;Encrypt=True;Connection Timeout=30;MultipleActiveResultSets=true;",
                    null)
            };
            var builder = new ContainerBuilder();
            builder.Register(_ => configuration).As<IConfigurationKeys>();
            builder.RegisterAssemblyTypes(typeof(IQueryHandler<,>).Assembly).AsClosedTypesOf(typeof(IQueryHandler<,>));
            builder.RegisterType<QueryBus>().As<IQueryBus>();
            builder.RegisterType<DapperRepository>().AsSelf();
            var _container = builder.Build();
            _queryBus = _container.Resolve<IQueryBus>();
            // _autoMock = AutoMock.GetLoose();

        }
        [Fact]
        public async Task DocumentAggregateQuery_Ok()
        {
            var query = new DocumentAggregateQuery(638, 0);

            var result = await _queryBus.QueryAsync(query, default);







        }
    }
}