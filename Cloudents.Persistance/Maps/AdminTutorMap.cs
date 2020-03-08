using Cloudents.Core.Entities;
using FluentNHibernate.Mapping;

namespace Cloudents.Persistence.Maps
{
    public class AdminTutorMap : ClassMap<AdminTutor>
    {
        public AdminTutorMap()
        {
            Id(x => x.Id).GeneratedBy.GuidComb();
            References(x => x.Tutor).Column("TutorId").Not.Nullable().ForeignKey("FK_admin_tutor");
        }
    }
}
