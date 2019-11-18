using Cloudents.Web.Binders;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Cloudents.Web.Models
{
    public class ChatMessageRequest
    {
        //public Guid? ChatId { get; set; }

        [Required, JsonConverter(typeof(StringHtmlEncoderConverter))]
        public string Message { get; set; }

        public long OtherUser { get; set; }
    }


    public class ChatResetRequest
    {
        [Required, Range(1, long.MaxValue)]
        public long OtherUserId { get; set; }
    }
}
