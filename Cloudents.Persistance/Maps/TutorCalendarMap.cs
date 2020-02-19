using Cloudents.Core.Entities;
using FluentNHibernate.Mapping;

namespace Cloudents.Persistence.Maps
{

    public class TutorCalendarMap : ClassMap<TutorCalendar>
    {
        public TutorCalendarMap()
        {
            Id(x => x.Id);

            Component(x => x.Calendar, y =>
            {
                y.Map(x => x.GoogleId).Not.Nullable();
                y.Map(x => x.Name).Not.Nullable();
            });

            References(x => x.Tutor).Not.Nullable();
        }
    }
}