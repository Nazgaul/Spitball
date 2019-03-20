using System.ComponentModel.DataAnnotations;

namespace Cloudents.Web.Models
{
    public class ChatMessageRequest
    {
        //public Guid? ChatId { get; set; }

        [Required]
        public string Message { get; set; }

        public long OtherUser { get; set; }
    }
}
