using Cloudents.Core.Entities;
using FluentNHibernate.Mapping;

namespace Cloudents.Persistence.Maps
{
    public class CourseEnrollmentMap : ClassMap<CourseEnrollment>
    {
        public CourseEnrollmentMap()
        {
            Id(x => x.Id).GeneratedBy.Guid();
            References(x => x.Course).Not.Nullable();
            References(x => x.User).Not.Nullable();

            Map(x => x.Receipt);
            Map(x => x.Create).Not.Nullable();
            Map(x => x.Price).CustomType<MoneyCompositeUserType>();

        }
    }
}
