using System;
using Cloudents.Web.Binders;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Cloudents.Web.Models
{
    public class ChatMessageRequest
    {

        [Required, JsonConverter(typeof(StringHtmlEncoderConverter))]
        public string Message { get; set; }

        public long OtherUser { get; set; }

        public Guid? ConversationId { get; set; }
    }

    public class ChatMessageResponse
    {
        public ChatMessageResponse(Guid conversationId)
        {
            ConversationId = conversationId;
        }

        public Guid ConversationId { get; }
    }


    public class ChatResetRequest
    {
        [Required]
        public Guid ConversationId { get; set; }
    }
}
