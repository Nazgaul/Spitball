using System.ComponentModel.DataAnnotations;

namespace Zbang.Cloudents.MobileApp.DataObjects
{
    public class InviteToBoxRequest
    {
        [Required]
        public string[] Recipients { get; set; }

        [Required]
        public long BoxId { get; set; }
    }

    public class InviteToSystemRequest
    {
        [Required]
        public string[] Recipients { get; set; }
    }
}