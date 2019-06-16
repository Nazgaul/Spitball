using Cloudents.Core.Entities;
using FluentNHibernate.Mapping;
using JetBrains.Annotations;

namespace Cloudents.Persistence.Maps
{
    [UsedImplicitly]
    public class ChatRoomMap : ClassMap<ChatRoom>
    {
        public ChatRoomMap()
        {
            Id(x => x.Id).GeneratedBy.GuidComb();
            Map(x => x.UpdateTime).Not.Nullable();
            Map(x => x.Identifier).Not.Nullable().Unique();
            Map(x => x.Status);
            HasMany(x => x.Users).Cascade.AllDeleteOrphan()
                .Inverse()
                .ForeignKeyConstraintName("fChatUserChatRoom")
                .KeyColumn("ChatRoomId");

            HasMany(x => x.Messages).Cascade.AllDeleteOrphan()
                .Inverse()
                .ForeignKeyConstraintName("fChatMessageChatRoom")
                .KeyColumn("ChatRoomId");
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
            Version(x => x.Version).CustomSqlType("rowversion").Generated.Always();
        }
    }

    public class ChatMessageMap : ClassMap<ChatMessage>
    {
        public ChatMessageMap()
        {
            Id(x => x.Id).GeneratedBy.GuidComb();
            Map(x => x.CreationTime).Not.Nullable();
            References(x => x.User).Not.Nullable().Column("UserId").ForeignKey("fChatUserUser");
            References(x => x.ChatRoom)
                .Not.Nullable().Column("ChatRoomId")
                .ForeignKey("fChatUserChatRoom");

            DiscriminateSubClassesOnColumn("MessageType");
            SchemaAction.Validate();
        }
    }

    public class ChatTextMessageMap : SubclassMap<ChatTextMessage>
    {
        public ChatTextMessageMap()
        {
            Map(x => x.Message).Length(8000);
            DiscriminatorValue("text");
        }
    }

    public class ChatAttachmentMessageMap : SubclassMap<ChatAttachmentMessage>
    {
        public ChatAttachmentMessageMap()
        {
            Map(x => x.Blob).Length(8000);
            DiscriminatorValue("attachment");
        }
    }
}