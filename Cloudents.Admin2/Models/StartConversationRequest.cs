using System.ComponentModel.DataAnnotations;

namespace Cloudents.Admin2.Models
{
    public class StartConversationRequest
    {
        public long UserId { get; set; }
        public long TutorId { get; set; }

        [Required] public string Message { get; set; }
    }
}