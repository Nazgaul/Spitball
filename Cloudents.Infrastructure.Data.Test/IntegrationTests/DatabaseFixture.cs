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
                    "Server=tcp:on0rodxe8f.database.windows.net;Database=ZBoxNew_Develop;User ID=ZBoxAdmin@on0rodxe8f;Password=Pa$$W0rd;Trusted_Connection=False;Encrypt=True;Connection Timeout=30;MultipleActiveResultSets=true;",
                    null)
            };
            var builder = new ContainerBuilder();
            builder.Register(_ => configuration).As<IConfigurationKeys>();
            builder.RegisterAssemblyTypes(typeof(IQueryHandler<,>).Assembly).AsClosedTypesOf(typeof(IQueryHandler<,>));
            builder.RegisterType<QueryBus>().As<IQueryBus>();
            builder.RegisterModule<ModuleDb>();
            builder.RegisterModule<ModuleCore>();
            builder.RegisterType<Persistence.DapperRepository>().AsSelf();
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