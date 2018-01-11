using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cloudents.Mobile.Models
{
    public class ItemLikeRequest
    {
        [Required, Range(1, long.MaxValue)]
        public long Id { get; set; }

        [Required]
        public IEnumerable<string> Tags { get; set; }
    }

    public class TagsRequest
    {
        public IEnumerable<string> Tags { get; set; }
    }
}