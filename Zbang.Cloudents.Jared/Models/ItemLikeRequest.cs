using System.ComponentModel.DataAnnotations;

namespace Zbang.Cloudents.Jared.Models
{
    public class ItemLikeRequest
    {
        [Required]
        public long Id { get; set; }

        [Required]
        public long BoxId { get; set; }
      
    }
}