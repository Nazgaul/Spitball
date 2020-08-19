using Cloudents.Web.Binders;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Cloudents.Web.Models
{
    public class ChatMessageRequest
    {

        [Required, JsonConverter(typeof(StringHtmlEncoderConverter))]
        public string Message { get; set; }


        [Required]
        public string ConversationId { get; set; }

        public long? TutorId { get; set; }
    }
}
