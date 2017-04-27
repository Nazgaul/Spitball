using System.ComponentModel.DataAnnotations;

namespace Zbang.Cloudents.Jared.DataObjects
{
    public class FollowRequest
    {
        [Required]
        public long BoxId { get; set; }
    }
}