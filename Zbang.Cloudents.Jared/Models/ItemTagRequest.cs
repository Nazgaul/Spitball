using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Zbang.Cloudents.Jared.Models
{
    public class ItemTagRequest
    {
        [Required]
        public long ItemId { get; set; }
        [Required]
        public IEnumerable<string> Tags { get; set; }
    }
}