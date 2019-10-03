using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using FluentNHibernate.Testing;
using Xunit;

namespace Cloudents.Infrastructure.Data.Test
{
    public class UniversityTests : InMemoryDatabaseTest
    {
        [Fact(Skip = "For future use")]
        public void CanCorrectlyMapUniversity()
        {
            new PersistenceSpecification<University>(Session)
                .CheckProperty(c => c.Name, "Some University")
                .CheckProperty(c => c.Country, "IL")
                .CheckProperty(c => c.State, ItemState.Pending)
                .VerifyTheMappings();
        }
    }
}