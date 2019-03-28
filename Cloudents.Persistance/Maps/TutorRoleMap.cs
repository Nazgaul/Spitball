using Cloudents.Core.Entities;
using FluentNHibernate.Mapping;

namespace Cloudents.Persistence.Maps
{
    public class TutorRoleMap : ClassMap<Tutor>
    {
        public TutorRoleMap()
        {
            Id(x => x.UserId).GeneratedBy.Assigned();
            Map(x => x.Bio).Length(1000);
            Map(x => x.Price).CustomSqlType("smallMoney");
           /* HasMany(x => x.Courses)
              .Table("TutorsCourses")
              .KeyColumn("TutorId")              
              .LazyLoad()
              .Inverse().Cascade.AllDeleteOrphan()
              .AsSet();
              */
            //Table("Tutor");
        }
    }
}