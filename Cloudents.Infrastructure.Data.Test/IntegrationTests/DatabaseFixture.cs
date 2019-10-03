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
        private IContainer container { get; set; }
        public DatabaseFixture()
        {
            var configuration = new ConfigurationKeys()
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
            container = builder.Build();
            QueryBus = container.Resolve<IQueryBus>();
            DapperRepository = container.Resolve<IDapperRepository>();
            TutorRepository = container.Resolve<ITutorRepository>();
            ReadTutorRepository = container.Resolve<IReadTutorRepository>();

            // ... initialize data in the test database ...
        }

        public void Dispose()
        {
            container.Dispose();
            // ... clean up test data from the database ...
        }

        public IQueryBus QueryBus { get; private set; }
        public IDapperRepository DapperRepository { get; }
        public ITutorRepository TutorRepository { get; private set; }
        public IReadTutorRepository ReadTutorRepository { get; set; }
    }
}