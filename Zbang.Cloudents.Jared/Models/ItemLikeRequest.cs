using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Zbang.Cloudents.Jared.Models
{
    public class ItemLikeRequest
    {
        [Required, Range(1, long.MaxValue)]
        public long Id { get; set; }

        [Required,Range(1,long.MaxValue)]
        public long BoxId { get; set; }

        [Required]
        public IEnumerable<string> Tags { get; set; }

    }
}