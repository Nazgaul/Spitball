using Cloudents.Core.Entities.Db;
using FluentNHibernate.Testing;
using Xunit;

namespace Cloudents.Infrastructure.Data.Test
{
    public class UrlStatsTests : InMemoryDatabaseTest
    {
        [Fact]
        public void CanCorrectlyMapUrlStats()
        {
            new PersistenceSpecification<UrlStats>(Session)
                .CheckProperty(c => c.AggregateCount, 1)
                .VerifyTheMappings();
        }
    }
}
