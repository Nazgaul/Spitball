//using FluentNHibernate.Mapping;
//using Cloudents.Core.Entities;
//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace Cloudents.Persistence.Maps
//{
//    public class TimeFrameMap : ClassMap<TimeFrame>
//    {
//        public TimeFrameMap()
//        {
//            Id(x => x.Id).GeneratedBy.GuidComb();
//            References(x => x.TutorHours).Column("TutorHoursId").ForeignKey("TimeFrame_TutorHours").Not.Nullable();
//            Map(x => x.From).CustomType<NHibernate.Type.TimeAsTimeSpanType>();
//            Map(x => x.To).CustomType<NHibernate.Type.TimeAsTimeSpanType>();
//            Table("TimeFrame");
//            SchemaAction.Update();
//        }
//    }
//}
