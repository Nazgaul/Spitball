using Cloudents.Core.Entities;
using FluentNHibernate.Mapping;

namespace Cloudents.Persistence.Maps
{
    public class TutorRoleMap : SubclassMap<Tutor>
    {
        public TutorRoleMap()
        {
            Map(x => x.Bio).Length(1000);
            Map(x => x.Price).CustomSqlType("smallMoney");
            Table("UserTutor");
        }
    }
}