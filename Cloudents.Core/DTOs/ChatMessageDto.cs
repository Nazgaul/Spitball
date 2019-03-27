using System;

namespace Cloudents.Core.DTOs
{
    public class ChatTextMessageDto : ChatMessageDto
    {
        public string Text { get; set; }

    }

    public abstract class ChatMessageDto
    {
        public long UserId { get; set; }
        public DateTime DateTime { get; set; }


    }

    public class ChatAttachmentDto : ChatMessageDto
    {
        public string File { get; set; }
    }
}