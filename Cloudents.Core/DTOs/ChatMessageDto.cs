using System;
using System.Runtime.Serialization;

namespace Cloudents.Core.DTOs
{
    [DataContract]
    public class ChatTextMessageDto : ChatMessageDto
    {
        [DataMember]
        public string Text { get; set; }
        [DataMember]
        public override string Type => "text";
    }

    [DataContract]
    public abstract class ChatMessageDto
    {
        [DataMember]
        public long UserId { get; set; }
        [DataMember] public DateTime DateTime { get; set; }

        [DataMember] public abstract string Type { get; }
        [DataMember] public string Name { get; set; }
        [DataMember] public string? Image { get; set; }
        [DataMember] public bool Unread { get; set; }
    }

    [DataContract]
    public class ChatAttachmentDto : ChatMessageDto
    {
        public Guid Id { get; set; }
        public Guid ChatRoomId { get; set; }

        [DataMember]
        public string Src { get; set; }
        [DataMember]
        public string Href { get; set; }

        public string Attachment { get; set; }

        [DataMember] public override string Type => "file";

    }
}