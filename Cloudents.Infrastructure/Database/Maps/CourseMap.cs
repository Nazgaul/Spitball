using Cloudents.Core.Entities.Db;
using Cloudents.Core.Enum;
using FluentNHibernate.Mapping;

namespace Cloudents.Infrastructure.Database.Maps
{
    public class CourseMap : ClassMap<Course>
    {
        public CourseMap()
        {
            Id(x => x.Id).Column("BoxId").GeneratedBy.Native();
            Map(e => e.Name).Length(255).Column("BoxName");
            Map(e => e.IsDeleted);
            //Map(e => e.IsDirty);
            Map(e => e.CourseCode).Length(255);
            Map(e => e.Discriminator).CustomType<CourseType>();
            Map(e => e.PrivacySetting).CustomType<CoursePrivacySetting>();


            Component(x => x.RowDetail, m =>
            {
                m.Map(x => x.CreatedUser).Insert().Not.Update();
                m.Map(x => x.CreationTime).Insert().Not.Update();
                m.Map(x => x.UpdateTime).Insert();
                m.Map(x => x.UpdatedUser).Insert();
            });
            References(e => e.University).Column(nameof(University)).ForeignKey("UniversityBoxes");
            Table("Box");
            Schema("Zbox");
        }
    }
}
