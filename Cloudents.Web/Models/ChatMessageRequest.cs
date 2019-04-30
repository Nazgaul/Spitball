using System.ComponentModel.DataAnnotations;
using Cloudents.Web.Binders;
using Newtonsoft.Json;

namespace Cloudents.Web.Models
{
    public class ChatMessageRequest
    {
        //public Guid? ChatId { get; set; }

        [Required,JsonConverter(typeof(StringHtmlEncoderConverter))]
        public string Message { get; set; }

        public long OtherUser { get; set; }
    }


    public class ChatResetRequest
    {
        public long OtherUserId { get; set; }
    }
}
