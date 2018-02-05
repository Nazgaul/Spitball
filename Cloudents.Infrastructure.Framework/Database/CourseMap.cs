using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Enum;
using FluentNHibernate;
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
}
