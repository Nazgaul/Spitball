using Autofac;
using Cloudents.Core;
using Cloudents.Core.Interfaces;
using Cloudents.Infrastructure.Stuff;
using Cloudents.Persistence;
using Cloudents.Query;
using System;

namespace Cloudents.Infrastructure.Data.Test.IntegrationTests
{
    public class DatabaseFixture : IDisposable
    {
        private IContainer Container { get; }
        public DatabaseFixture()
        {
            var configuration = new ConfigurationKeys()
            {
                Db = new DbConnectionString(
                    "Server=tcp:sb-dev.database.windows.net,1433;Initial Catalog=ZboxNew_Develop;Persist Security Info=False;User ID=sb-dev;Password=Pa$$W0rd123;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;",
                    null, DbConnectionString.DataBaseIntegration.None)


                //PROD
                //Db = new DbConnectionString(
                //"Server=tcp:on0rodxe8f.database.windows.net,1433;Initial Catalog=ZboxNew;Persist Security Info=False;User ID=ZBoxAdmin@on0rodxe8f;Password=Pa$$W0rd;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;",
                //null, DbConnectionString.DataBaseIntegration.None)

            };
            var builder = new ContainerBuilder();
            builder.Register(_ => configuration).As<IConfigurationKeys>();
            //builder.RegisterAssemblyTypes(typeof(IQueryHandler<,>).Assembly).AsClosedTypesOf(typeof(IQueryHandler<,>));
            builder.RegisterType<QueryBus>().As<IQueryBus>();
            builder.RegisterModule<ModuleDb>();
            builder.RegisterModule<ModuleCore>();
            builder.RegisterModule<ModuleInfrastructureBase>();
            // builder.RegisterModule<ModuleCache>();

            builder.RegisterType<DapperRepository>().AsSelf();
            Container = builder.Build();
            QueryBus = Container.Resolve<IQueryBus>();
            DapperRepository = Container.Resolve<IDapperRepository>();
            TutorRepository = Container.Resolve<ITutorRepository>();
            ReadTutorRepository = Container.Resolve<IReadTutorRepository>();

            // ... initialize data in the test database ...
        }

        public void Dispose()
        {
            Container.Dispose();
            // ... clean up test data from the database ...
        }

        public IQueryBus QueryBus { get; }
        public IDapperRepository DapperRepository { get; }
        public ITutorRepository TutorRepository { get; }
        public IReadTutorRepository ReadTutorRepository { get; }
    }
}