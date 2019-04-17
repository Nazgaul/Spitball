﻿using Cloudents.Core.Entities;
using FluentNHibernate.Mapping;

namespace Cloudents.Persistence.Maps
{
    public class StudyRoomSessionMap : ClassMap<StudyRoomSession>
    {
        public StudyRoomSessionMap()
        {
            Id(x => x.Id).GeneratedBy.GuidComb();
            References(x => x.StudyRoom).Column("StudyRoomId").ForeignKey("Session_Room").Not.Nullable();
            Map(x => x.Created).Not.Nullable();
            Map(x => x.Ended);
            Map(x => x.Duration);
            Map(x => x.SessionId).Not.Nullable();
            SchemaAction.Update();
           
        }
    }
}
