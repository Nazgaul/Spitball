using Cloudents.Core.Entities.Db;
using FluentNHibernate.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cloudents.Infrastructure.Framework.Test
{
    [TestClass]
    public class UniversityTests : InMemoryDatabaseTest
    {
        [TestMethod]
        public void CanCorrectlyMapUniversity()
        {
            new PersistenceSpecification<University>(Session)
                .CheckProperty(c => c.Name, "Some University")
                .CheckProperty(c => c.Id, 171885L)
                .VerifyTheMappings();
        }
    }
}