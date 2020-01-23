using Cloudents.Core.Entities;
using FluentNHibernate.Mapping;
using System;

namespace Cloudents.Persistence.Maps
{
    public class TutorHoursMap : ClassMap<TutorHours>
    {
        public TutorHoursMap()
        {
            Id(x => x.Id).GeneratedBy.GuidComb();
            References(x => x.Tutor).Column("TutorId").ForeignKey("Tutor_Hours").Not.Nullable().UniqueKey("tutor_day");
            Component(x => x.AvailabilitySlot, z => {
                z.Map(x => x.Day).Column("WeekDay").CustomType<GenericEnumStringType<DayOfWeek>>().UniqueKey("tutor_day");
                z.Map(x => x.From).Column("From").Not.Nullable().CustomType<NHibernate.Type.TimeAsTimeSpanType>().UniqueKey("tutor_day");
                z.Map(x => x.To).Column("To").Not.Nullable().CustomType<NHibernate.Type.TimeAsTimeSpanType>().UniqueKey("tutor_day");
            });
            Map(x => x.CreateTime).Not.Update();
            Table("TutorHours");
            SchemaAction.Update();
        }
    }


}
