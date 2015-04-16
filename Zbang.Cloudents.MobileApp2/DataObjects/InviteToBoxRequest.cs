using System.ComponentModel.DataAnnotations;

namespace Zbang.Cloudents.MobileApp2.DataObjects
{
    public class InviteToBoxRequest
    {
        [Required]
        public string[] Recipients { get; set; }

        [Required]
        public long BoxId { get; set; }
    }
}