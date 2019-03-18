using Cloudents.Core.Entities;
using FluentNHibernate.Mapping;

namespace Cloudents.Persistence.Maps
{
    public class ChatRoomMap : ClassMap<ChatRoom>
    {
        public ChatRoomMap()
        {
            Id(x => x.Id).GeneratedBy.GuidComb();
            Map(x => x.UpdateTime).Not.Nullable();
            Map(x => x.Identifier).Not.Nullable().Unique();
            HasMany(x => x.Users).Cascade.AllDeleteOrphan()
                .Inverse()
                .ForeignKeyConstraintName("fChatUserChatRoom")
                .KeyColumn("ChatRoomId");
            SchemaAction.Update();
        }
    }

    public class ChatUserMap : ClassMap<ChatUser>
    {
        public ChatUserMap()
        {
            Id(x => x.Id).GeneratedBy.GuidComb();
            Map(x => x.Unread).Not.Nullable();
            References(x => x.ChatRoom)
                .Not.Nullable().Column("ChatRoomId").ForeignKey("fChatUserChatRoom");
            References(x => x.User)
                .Not.Nullable().Column("UserId")
                .ForeignKey("fChatUserUser");

            HasMany(x => x.Messages).Cascade.AllDeleteOrphan()
                .Inverse()
                .ForeignKeyConstraintName("fChatUserUser")
                .KeyColumn("UserId");

            SchemaAction.Update();
        }
    }

    public class ChatMessageMap : ClassMap<ChatMessage>
    {
        public ChatMessageMap()
        {
            Id(x => x.Id).GeneratedBy.GuidComb();
            Map(x => x.Message).Length(8000);
            Map(x => x.CreationTime).Not.Nullable();
            References(x => x.User).Not.Nullable().Column("UserId").ForeignKey("fChatUserUser");
            SchemaAction.Update();
        }
    }
}