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

        public string ConversationId { get; set; }
    }


    public class ChatResetRequest
    {
        [Required]
        public string ConversationId { get; set; }
    }
}
