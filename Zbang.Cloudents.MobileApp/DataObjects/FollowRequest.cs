using System.ComponentModel.DataAnnotations;

namespace Zbang.Cloudents.MobileApp.DataObjects
{
    public class FollowRequest
    {
        [Required]
        public long BoxId { get; set; }
    }
}