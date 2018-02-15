using System;
using Cloudents.Core.Entities.Db;
using FluentNHibernate.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHibernate;

namespace Cloudents.Infrastructure.Framework.Test
{
    [TestClass]
    public class UnitTest1 : InMemoryDatabaseTest
    {
        [TestMethod]
        public void CanCorrectlyMapUrlStats()
        {
            new PersistenceSpecification<UrlStats>(session)
                .CheckProperty(c => c.AggregateCount, 1)
                .VerifyTheMappings();
        }

    }
}
