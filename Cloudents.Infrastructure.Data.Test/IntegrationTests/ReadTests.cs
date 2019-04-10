using System;
using System.Data.SqlClient;
using Autofac;
using Cloudents.Core;
using Cloudents.Core.Interfaces;
using Cloudents.Query;
using Cloudents.Query.Documents;
using System.Threading.Tasks;
using Cloudents.Infrastructure.Stuff;
using Cloudents.Persistence;
using Xunit;

namespace Cloudents.Infrastructure.Data.Test.IntegrationTests
{

    public class DatabaseFixture : IDisposable
    {

        public DatabaseFixture()
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
            builder.RegisterModule<ModuleDb>();
            builder.RegisterModule<ModuleCore>();
            builder.RegisterType<DapperRepository>().AsSelf();
            var _container = builder.Build();
            _queryBus = _container.Resolve<IQueryBus>();

            // ... initialize data in the test database ...
        }

        public void Dispose()
        {
            // ... clean up test data from the database ...
        }

        public IQueryBus _queryBus { get; private set; }
    }
    [CollectionDefinition("Database collection")]
    public class DatabaseCollection : ICollectionFixture<DatabaseFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }

    [Collection("Database collection")]
    public class ReadTests
    {
        //private readonly DapperRepository _dapperRepository;
        //private readonly AutoMock _autoMock;
        //private readonly IQueryBus _queryBus;
        // private readonly IContainer _container;
        DatabaseFixture fixture;

        public ReadTests(DatabaseFixture fixture)
        {
            this.fixture = fixture;
            // _autoMock = AutoMock.GetLoose();

        }
        [Fact]
        public async Task DocumentAggregateQuery_Ok()
        {
            var query = new DocumentAggregateQuery(638, 0,null);

            var result = await fixture._queryBus.QueryAsync(query, default);
        }

        [Fact]
        public async Task DocumentAggregateQuery_WithFilter_Ok()
        {
            var query = new DocumentAggregateQuery(638, 0, new []{"x", "y" });

            var result = await fixture._queryBus.QueryAsync(query, default);
        }

        [Fact]
        public async Task DocumentCourseQuery_Ok()
        {
            var query = new DocumentCourseQuery(638, 0, "economics",  null);

            var result = await fixture._queryBus.QueryAsync(query, default);
        }


        [Fact]
        public async Task DocumentCourseQuery_Filter_Ok()
        {
            var query = new DocumentCourseQuery(638, 0, "economics", new[] { "x", "y" });

            var result = await fixture._queryBus.QueryAsync(query, default);
        }
    }
}