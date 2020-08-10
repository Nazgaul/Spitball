﻿using Cloudents.Core.Entities;
using FluentNHibernate.Mapping;

namespace Cloudents.Persistence.Maps
{
    public class StudyRoomSessionMap : ClassMap<StudyRoomSession>
    {
        public StudyRoomSessionMap()
        {
            Id(x => x.Id).GeneratedBy.GuidComb();
            References(x => x.StudyRoom).Column("StudyRoomId")
                .ForeignKey("Session_Room").Not.Nullable().LazyLoad(Laziness.NoProxy);
            Map(x => x.Created).Not.Nullable();
            Map(x => x.Ended);
            //Map(x => x.RejoinCount);
            Map(x => x.Duration);
            Map(x => x.DurationTicks).Column("Duration").ReadOnly();
          //  Map(x => x.RealDuration);
            Map(x => x.SessionId).Not.Nullable();
            Map(x => x.Receipt);
            Map(x => x.Price).CustomSqlType("smallMoney").Nullable();
            //Map(x => x.VideoExists);
            HasMany(x => x.RoomSessionUsers).Access.CamelCaseField(Prefix.Underscore)
               .KeyColumn("SessionId")
               .Inverse().Cascade.AllDeleteOrphan().AsSet();
           // Map(m => m.PaymentApproved).Nullable();
           // Map(x => x.AdminDuration).Column("AdminDuration2").Nullable();
            Map(x => x.StudyRoomVersion);
            Version(x => x.Version).CustomSqlType("timestamp").Generated.Always();
        }
    }
}
