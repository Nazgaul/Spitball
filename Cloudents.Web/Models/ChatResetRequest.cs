using System.ComponentModel.DataAnnotations;

namespace Cloudents.Web.Models
{
    public class ChatResetRequest
    {
        [Required]
        public string ConversationId { get; set; }
    }
}