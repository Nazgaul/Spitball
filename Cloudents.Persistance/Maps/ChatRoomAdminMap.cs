using Cloudents.Core.Entities;
using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cloudents.Persistence.Maps
{
    public class ChatRoomAdminMap : ClassMap<ChatRoomAdmin>
    {
        public ChatRoomAdminMap()
        {
            Id(x => x.Id).GeneratedBy.Foreign("ChatRoom");
            Map(x => x.Status);
            Map(x => x.AssignTo);
            SchemaAction.Update();
        }
    }
}
