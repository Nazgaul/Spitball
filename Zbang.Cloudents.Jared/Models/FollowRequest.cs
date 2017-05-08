using System.ComponentModel.DataAnnotations;

namespace Zbang.Cloudents.Jared.Models
{
    public class FollowRequest
    {
        [Required]
        public long BoxId { get; set; }
    }
}