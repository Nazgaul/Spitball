using System.ComponentModel.DataAnnotations;

namespace Cloudents.Mobile.Models
{
    public class FollowRequest
    {
        [Required]
        public long BoxId { get; set; }
    }
}