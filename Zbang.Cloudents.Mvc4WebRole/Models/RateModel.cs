
using System.ComponentModel.DataAnnotations;


namespace Zbang.Cloudents.Mvc4WebRole.Models
{
    public class RateModel
    {
        [Required]
        public long ItemId { get; set; }
        [Required]
        public int Rate { get; set; }

        [Required]
        public long BoxId { get; set; }
    }
}