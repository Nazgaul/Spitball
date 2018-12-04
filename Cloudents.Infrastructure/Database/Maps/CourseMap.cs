﻿using System.Diagnostics.CodeAnalysis;
using Cloudents.Core.Entities.Db;
using FluentNHibernate.Mapping;

namespace Cloudents.Infrastructure.Database.Maps
{
    [SuppressMessage("ReSharper", "UnusedMember.Global",Justification = "Fluent nhibernate")]
    public sealed class CourseMap : ClassMap<Course>
    {
        public CourseMap()
        {
            Id(e => e.Name).GeneratedBy.Assigned().Length(150);
            Map(x => x.Count).Not.Nullable();

            SchemaAction.None();
        }
    }
}
