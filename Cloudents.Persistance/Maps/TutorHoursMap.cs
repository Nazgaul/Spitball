using Cloudents.Core.Entities;
using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cloudents.Persistence.Maps
{
    public class TutorHoursMap : ClassMap<TutorHours>
    {
        public TutorHoursMap()
        {
            Id(x => x.Id).GeneratedBy.GuidComb();
            References(x => x.Tutor).Column("TutorId").ForeignKey("Tutor_Hours").Not.Nullable().UniqueKey("tutor_day");
            Map(x => x.WeekDay).CustomType<GenericEnumStringType<DayOfWeek>>().UniqueKey("tutor_day");
            Map(x => x.From).Not.Nullable().CustomType<NHibernate.Type.TimeAsTimeSpanType>().UniqueKey("tutor_day");
            Map(x => x.To).Not.Nullable().CustomType<NHibernate.Type.TimeAsTimeSpanType>().UniqueKey("tutor_day");
            //HasMany(x => x.TimeFrames).Access.CamelCaseField(Prefix.Underscore)
            //        .Cascade.AllDeleteOrphan().Inverse().AsSet();
            Map(x => x.CreateTime).Not.Update();
            Table("TutorHours");
            SchemaAction.Update();
        }
    }


}
