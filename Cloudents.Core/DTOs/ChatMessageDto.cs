using System;

namespace Cloudents.Core.DTOs
{
    public class ChatMessageDto
    {
        public long UserId { get; set; }
        public string Text { get; set; }

        public DateTime DateTime { get; set; }
    }
}