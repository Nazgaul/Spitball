using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cloudents.Mobile.Models
{
    public class ItemTagRequest
    {
        [Required]
        public long Id { get; set; }
        [Required]
        public IEnumerable<string> Tags { get; set; }
    }
}