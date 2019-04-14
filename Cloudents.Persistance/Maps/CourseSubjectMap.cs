using Cloudents.Core.Entities;
using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cloudents.Persistence.Maps
{
    public sealed class CourseSubjectMap : ClassMap<CourseSubject>
    {
        public CourseSubjectMap()
        {
            Id(x => x.Id).GeneratedBy.Identity();
            Map(e => e.Name).Not.Nullable().Unique().Length(150);
            Table("CourseSubject");
            ReadOnly();
            SchemaAction.Validate();
        }
    }
}
