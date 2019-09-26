using Cloudents.Core.Entities;
using FluentNHibernate.Mapping;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Mapping.ByCode;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cloudents.Persistence.Maps
{
    public class TutorHoursMap : ClassMapping<TutorHours>
    {
        public TutorHoursMap()
        {
            Id(x => x.Id, c => c.Generator(Generators.GuidComb));
            //Id(x => x.Id).GeneratedBy.GuidComb();
            ManyToOne(x => x.Tutor, c => {
                c.Column("TutorId");
                c.ForeignKey("Tutor_Hours");
                c.NotNullable(true);
                c.UniqueKey("tutor_day");
            });
            //References(x => x.Tutor).Column("TutorId").ForeignKey("Tutor_Hours").Not.Nullable().UniqueKey("tutor_day");
            Property(x => x.WeekDay, c => {
                c.UniqueKey("tutor_day");
                c.Type<GenericEnumStringType<DayOfWeek>>();
            });
            //Map(x => x.WeekDay).CustomType<GenericEnumStringType<DayOfWeek>>().UniqueKey("tutor_day");
            Property(x => x.From, c => {
                c.NotNullable(true);
                c.Type<NHibernate.Type.TimeAsTimeSpanType>();
                c.UniqueKey("tutor_day");
            });
            //Map(x => x.From).Not.Nullable().CustomType<NHibernate.Type.TimeAsTimeSpanType>().UniqueKey("tutor_day");
            Property(x => x.To, c => {
                c.NotNullable(true);
                c.Type<NHibernate.Type.TimeAsTimeSpanType>();
                c.UniqueKey("tutor_day");
            });
            //Map(x => x.To).Not.Nullable().CustomType<NHibernate.Type.TimeAsTimeSpanType>().UniqueKey("tutor_day");
            ////HasMany(x => x.TimeFrames).Access.CamelCaseField(Prefix.Underscore)
            ////        .Cascade.AllDeleteOrphan().Inverse().AsSet();
            Table("TutorHours");
            //Table("TutorHours");
            SchemaAction(NHibernate.Mapping.ByCode.SchemaAction.Update);
            //SchemaAction.Update();
        }
    }


}
