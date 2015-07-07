using System.ComponentModel.DataAnnotations;

namespace Zbang.Cloudents.MobileApp2.DataObjects
{
    public class FollowRequest
    {
        [Required]
        public long BoxId { get; set; }
    }
}