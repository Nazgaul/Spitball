﻿using Cloudents.Core.Entities;
using FluentNHibernate.Testing;
using Xunit;

namespace Cloudents.Infrastructure.Data.Test
{
    public class UniversityTests : InMemoryDatabaseTest
    {
        [Fact]
        public void CanCorrectlyMapUniversity()
        {
            new PersistenceSpecification<University>(Session)
                .CheckProperty(c => c.Name, "Some University")
                .CheckProperty(c => c.Id, 171885L)
                .VerifyTheMappings();
        }
    }
}