using Cloudents.Core.Entities;
//using FluentNHibernate.Mapping;
using JetBrains.Annotations;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Type;

namespace Cloudents.Persistence.Maps
{
    [UsedImplicitly]
    public class ChatRoomMap : ClassMapping<ChatRoom>
    {
        public ChatRoomMap()
        {
            Id(x => x.Id, c => c.Generator(Generators.GuidComb));
            //Id(x => x.Id).GeneratedBy.GuidComb();
            Property(x => x.UpdateTime, c => c.NotNullable(true));
            //Map(x => x.UpdateTime).Not.Nullable();
            Property(x => x.Identifier, c => { c.NotNullable(true); c.Unique(true); });
            //Map(x => x.Identifier).Not.Nullable().Unique();
            // One 
            OneToOne(x => x.Extra, c => c.Cascade(Cascade.All));
            //HasOne(x => x.Extra)/*.LazyLoad(Laziness.NoProxy).Constrained()*/.Cascade.All();
            Bag(x => x.Users, c => {
                c.Cascade(Cascade.All | Cascade.DeleteOrphans);
                c.Inverse(true);
                c.Key(k => {
                    k.Column("ChatRoomId");
                    k.ForeignKey("fChatUserChatRoom");
                    });
            }, a => a.OneToMany());
            //HasMany(x => x.Users).Cascade.AllDeleteOrphan()
            //    .Inverse()
            //    .ForeignKeyConstraintName("fChatUserChatRoom")
            //    .KeyColumn("ChatRoomId");

            Bag(x => x.Messages, c => {
                c.Cascade(Cascade.All | Cascade.DeleteOrphans);
                c.Inverse(true);
                c.Key(k => {
                    k.Column("ChatRoomId");
                    k.ForeignKey("fChatMessageChatRoom");
                });
            }, a => a.OneToMany());
            //HasMany(x => x.Messages).Cascade.AllDeleteOrphan()
            //    .Inverse()
            //    .ForeignKeyConstraintName("fChatMessageChatRoom")
            //    .KeyColumn("ChatRoomId");
        }
    }

    public class ChatUserMap : ClassMapping<ChatUser>
    {
        public ChatUserMap()
        {
            Id(x => x.Id, c => c.Generator(Generators.GuidComb));
            //Id(x => x.Id).GeneratedBy.GuidComb();
            Property(x => x.Unread, c => c.NotNullable(true));
            //Map(x => x.Unread).Not.Nullable();
            ManyToOne(x => x.ChatRoom, c =>
            {
                c.NotNullable(true);
                c.Column("ChatRoomId");
                c.ForeignKey("fChatUserChatRoom");
            });
            //References(x => x.ChatRoom)
            //    .Not.Nullable().Column("ChatRoomId").ForeignKey("fChatUserChatRoom");

            ManyToOne(x => x.User, c => {
                c.NotNullable(true);
                c.Column("UserId");
                c.ForeignKey("fChatUserUser");
            });
            //References(x => x.User)
            //    .Not.Nullable().Column("UserId")
            //    .ForeignKey("fChatUserUser");
            DynamicUpdate(true);
            //DynamicUpdate();
            OptimisticLock(OptimisticLockMode.Version);
            //OptimisticLock.Version();

            Version(x => x.Version, c => {
                c.Generated(VersionGeneration.Always);
                c.Type(new BinaryBlobType());
                c.Column(cl => {
                    cl.SqlType("timestamp");
                    //cl.NotNullable(false);
                });
            });

            //Version(x => x.Version).CustomSqlType("rowversion").Generated.Always();
        }
    }

    public class ChatMessageMap : ClassMapping<ChatMessage>
    {
        public ChatMessageMap()
        {
            Id(x => x.Id, c => {
                c.Generator(Generators.GuidComb);
            });
            //Id(x => x.Id).GeneratedBy.GuidComb();
            Property(x => x.CreationTime, c =>
            {
                c.NotNullable(true);
            });
            //Map(x => x.CreationTime).Not.Nullable();
            ManyToOne(x => x.User, c => {
                c.NotNullable(true);
                c.Column("UserId");
                c.ForeignKey("fChatUserUser");
            });
            //References(x => x.User).Not.Nullable().Column("UserId").ForeignKey("fChatUserUser");
            ManyToOne(x => x.ChatRoom, c=> {
                c.NotNullable(true);
                c.Column("ChatRoomId");
                c.ForeignKey("fChatUserChatRoom");
            });
            //References(x => x.ChatRoom)
            //    .Not.Nullable().Column("ChatRoomId")
            //    .ForeignKey("fChatUserChatRoom");

            Discriminator(d => d.Column("MessageType"));
            //DiscriminateSubClassesOnColumn("MessageType");
            SchemaAction(NHibernate.Mapping.ByCode.SchemaAction.Validate);
            //SchemaAction.Validate();
        }
    }

    public class ChatTextMessageMap : SubclassMapping<ChatTextMessage>
    {
        public ChatTextMessageMap()
        {
            Property(x => x.Message, c => {
                c.Length(8000);
            });
            //Map(x => x.Message).Length(8000);
            DiscriminatorValue("text");
        }
    }

    public class ChatAttachmentMessageMap : SubclassMapping<ChatAttachmentMessage>
    {
        public ChatAttachmentMessageMap()
        {
            Property(x => x.Blob, c => {
                c.Length(8000);
            });
            //Map(x => x.Blob).Length(8000);
            DiscriminatorValue("attachment");
        }
    }

    //public class ChatSystemMessageMap : SubclassMap<SystemTextMessage>
    //{
    //    public ChatSystemMessageMap()
    //    {
    //        //Map(x => x.Blob).Length(8000);
    //        DiscriminatorValue("system");
    //    }
    //}
}