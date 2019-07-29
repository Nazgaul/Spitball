using System;
using Autofac;
using Cloudents.Core;
using Cloudents.Core.Interfaces;
using Cloudents.Infrastructure.Stuff;
using Cloudents.Persistence;
using Cloudents.Query;

namespace Cloudents.Infrastructure.Data.Test.IntegrationTests
{
    public class DatabaseFixture : IDisposable
    {

        public DatabaseFixture()
        {
            var configuration = new ConfigurationKeys("SomeSite")
            {
                Db = new DbConnectionString(
                    "Server=tcp:sb-dev.database.windows.net,1433;Initial Catalog=ZboxNew_Develop;Persist Security Info=False;User ID=sb-dev;Password=Pa$$W0rd123;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;",
                    null)
            };
            var builder = new ContainerBuilder();
            builder.Register(_ => configuration).As<IConfigurationKeys>();
            builder.RegisterAssemblyTypes(typeof(IQueryHandler<,>).Assembly).AsClosedTypesOf(typeof(IQueryHandler<,>));
            builder.RegisterType<QueryBus>().As<IQueryBus>();
            builder.RegisterModule<ModuleDb>();
            builder.RegisterModule<ModuleCore>();
            builder.RegisterType<DapperRepository>().AsSelf();
            var container = builder.Build();
            QueryBus = container.Resolve<IQueryBus>();

            // ... initialize data in the test database ...
        }

        public void Dispose()
        {
            // ... clean up test data from the database ...
        }

        public IQueryBus QueryBus { get; private set; }
    }
}