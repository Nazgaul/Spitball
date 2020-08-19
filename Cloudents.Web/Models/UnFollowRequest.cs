using System.ComponentModel.DataAnnotations;

namespace Cloudents.Web.Models
{
    public class UnFollowRequest
    {
        [Required]
        public long Id { get; set; }
    }
}