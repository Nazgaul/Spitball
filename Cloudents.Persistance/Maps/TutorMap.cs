using Cloudents.Core.Entities;
using FluentNHibernate.Mapping;

namespace Cloudents.Persistence.Maps
{
    public class TutorMap : ClassMap<Tutor>
    {
        public TutorMap()
        {
            Id(x => x.Id).GeneratedBy.Foreign("User");
            //CompositeId()
            //    .KeyReference(x => x.User, "UserId");
            //Id(x => x.UserId).GeneratedBy.Assigned();
            HasOne(x => x.User).Constrained().Cascade.None();
            Map(x => x.Bio).Length(1000);
            Map(x => x.Price).CustomSqlType("smallMoney");

            Map(x => x.SellerKey);
            HasMany(x => x.Reviews).Access.CamelCaseField(Prefix.Underscore).Cascade.AllDeleteOrphan().Inverse();
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