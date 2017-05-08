using System;
using ProtoBuf;

namespace Zbang.Zbox.Infrastructure.Transport
{

    [ProtoContract]
    public class MessageMailData : BaseMailData
    {
        protected MessageMailData()
        {
        }

        public MessageMailData(string message, string emailAddress, string senderUserName,
            string senderImage,string senderEmail,
            string culture, long userId, Guid conversationId, long senderUserId)
            : base(emailAddress, culture)
        {
            SenderUserName = senderUserName;
            Message = message;
            SenderUserImage = senderImage;
            SenderUserEmail = senderEmail;
            UserId = userId;
            ConversationId = conversationId;
            SenderUserId = senderUserId;
        }
        [ProtoMember(1)]
        public string Message { get; private set; }
        [ProtoMember(2)]
        public string SenderUserName { get; private set; }
        [ProtoMember(3)]
        public string SenderUserImage { get; private set; }
        [ProtoMember(4)]
        public string SenderUserEmail { get; private set; }

        [ProtoMember(7)]
        public long? SenderUserId { get; private set; }

        [ProtoMember(5)]
        public long UserId { get; private set; }

        [ProtoMember(6)]
        public Guid ConversationId { get; private set; }



        public override string MailResolver => MessageResolver;
    }
}
