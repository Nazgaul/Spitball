using Autofac;
using Cloudents.Core;
using Cloudents.Core.Interfaces;
using Cloudents.Infrastructure;
using Cloudents.Infrastructure.Stuff;
using Cloudents.Persistence;
using Cloudents.Query;
using Cloudents.Search.Document;
using System;

namespace Cloudents.Search.Test
{
    public sealed class SearchFixture : IDisposable
    {
        private IContainer Container { get; }

        public SearchFixture()
        {
            var configuration = new ConfigurationKeys()
            {
                Db = new DbConnectionString(
                    "Server=tcp:sb-dev.database.windows.net,1433;Initial Catalog=ZboxNew_Develop;Persist Security Info=False;User ID=sb-dev;Password=Pa$$W0rd123;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;",
                    null, DbConnectionString.DataBaseIntegration.None),
                Storage =  "DefaultEndpointsProtocol=https;AccountName=spitballdev;AccountKey=fEzJ1MJZyIQMCoGRK/8lnwCHRLm3A3g0+ZnvoIxed7Bl5MlWw/FkbPKrDhCIlWzasWVCK6q0U4HQZ3qCLnXelg==;EndpointSuffix=core.windows.net"
            };
            var builder = new ContainerBuilder();
            builder.Register(_ => configuration).As<IConfigurationKeys>();
            builder.RegisterType<QueryBus>().As<IQueryBus>();
            builder.RegisterType<AzureDocumentSearch>().As<IDocumentsSearch>();
            builder.RegisterModule<ModuleDb>();
            builder.RegisterModule<ModuleCore>();
            builder.RegisterModule<ModuleInfrastructureBase>();

            builder.Register(c =>
            {
                return new SearchService("5B0433BFBBE625C9D60F7330CFF103F0", "cloudents", true);
            }).AsSelf().As<ISearchService>().SingleInstance();
           

            Container = builder.Build();

            DocumentsSearch = Container.Resolve<IDocumentsSearch>();
            QueryBus = Container.Resolve<IQueryBus>();
        }

        public void Dispose()
        {
            Container.Dispose();
        }
        public IDocumentsSearch DocumentsSearch { get; }
        public IQueryBus QueryBus { get; }
    }
}
