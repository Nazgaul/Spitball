using System.ComponentModel.DataAnnotations;

namespace Zbang.Cloudents.Jared.Models
{
    public class AddCommentRequest
    {
        [Required]
        public string Content { get; set; }
    }
}