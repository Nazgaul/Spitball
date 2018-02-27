using Cloudents.Core.Entities.Db;
using FluentNHibernate.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cloudents.Infrastructure.Framework.Test
{
    [TestClass]
    public class UrlStatsTests : InMemoryDatabaseTest
    {
        [TestMethod]
        public void CanCorrectlyMapUrlStats()
        {
            new PersistenceSpecification<UrlStats>(Session)
                .CheckProperty(c => c.AggregateCount, 1)
                .VerifyTheMappings();
        }

    }
}
