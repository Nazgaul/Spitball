using Cloudents.Core.Entities;
using FluentNHibernate.Mapping;

namespace Cloudents.Persistence.Maps
{
    public class TutorsCoursesMap : ClassMap<TutorsCourses>
    {
        public TutorsCoursesMap()
        {
            Id(x => x.Id).GeneratedBy.GuidComb();
            References(x => x.Tutor).Not.Nullable().Column("TutorId");
            References(x => x.Course).Not.Nullable().Column("CourseId");
            SchemaAction.Update();
        }
    }
}
