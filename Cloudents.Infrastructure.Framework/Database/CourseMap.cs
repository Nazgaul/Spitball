﻿using Cloudents.Core.Entities.Db;
using Cloudents.Core.Enum;
using FluentNHibernate.Mapping;

namespace Cloudents.Infrastructure.Framework.Database
{
    public class CourseMap : ClassMap<Course>
    {
        public CourseMap()
        {
            Id(x => x.Id).Column("BoxId").GeneratedBy.Native();
            Map(e => e.Name).Length(255).Column("BoxName");
            Map(e => e.IsDeleted);
            Map(e => e.IsDirty);
            Map(e => e.CourseCode).Length(255);
            Map(e => e.Discriminator).CustomType<CourseType>();
            Map(e => e.PrivacySetting).CustomType<CoursePrivacySetting>();

            Component(e => e.RowDetail);
            References(e => e.University).Column(nameof(University)).ForeignKey("UniversityBoxes");
            Table("Box");
            Schema("Zbox");
        }
    }

    public class UrlStatsMap : ClassMap<UrlStats>
    {
        public UrlStatsMap()
        {
            Id(x => x.Id).GeneratedBy.GuidComb();
            Map(e => e.Host);
            Map(e => e.DateTime);
            Map(e => e.UrlSource);
            Map(e => e.UrlTarget);
            Map(e => e.SourceLocation);
            Map(e => e.AggregateCount);
        }
    }
}
