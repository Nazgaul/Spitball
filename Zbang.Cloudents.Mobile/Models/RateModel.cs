using System.ComponentModel.DataAnnotations;

namespace Zbang.Cloudents.Mobile.Models
{
    public class RateModel
    {
        [Required]
        public long ItemId { get; set; }
        [Required]
        public int Rate { get; set; }
    }
}