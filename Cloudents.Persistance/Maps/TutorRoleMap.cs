using Cloudents.Core.Entities;
using FluentNHibernate.Mapping;

namespace Cloudents.Persistence.Maps
{
    public class TutorMap : ClassMap<Tutor>
    {
        public TutorMap()
        {
            //CompositeId()
           //.KeyReference(x => x.User, "UserId");
            Id(x => x.UserId).GeneratedBy.Assigned();
            Map(x => x.Bio).Not.Nullable().Length(1000);
            Map(x => x.Price).Not.Nullable().CustomSqlType("smallMoney");
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