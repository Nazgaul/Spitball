using Cloudents.Core.Entities;
using FluentNHibernate.Testing;
using Xunit;

namespace Cloudents.Infrastructure.Data.Test
{
    public class UniversityTests : InMemoryDatabaseTest
    {
        //[Fact]
        //public void CanCorrectlyMapUniversity()
        //{
        //    new PersistenceSpecification<University>(Session)
        //        .CheckProperty(c => c.Name, "Some University")
        //        .CheckProperty(c => c.Country, "IL")
        //        .CheckProperty(c => c.State, ItemState.Pending)
        //        .VerifyTheMappings();
        //}

        [Fact(Skip = "There is a problem generating the scheme")]
        public void CanCorrectlyMapUniversity()
        {
            new PersistenceSpecification<User>(Session)
                .CheckProperty(c => c.Name, "Some University")
                .CheckProperty(c => c.Country, "IL")
                // .CheckProperty(c => c.State, ItemState.Pending)
                .VerifyTheMappings();
        }
    }
}