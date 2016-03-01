using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Zbang.Cloudents.MobileApp.DataObjects
{
    public class ItemLikeRequest
    {
        [Required]
        public long Id { get; set; }
        [Required]
        public long BoxId { get; set; }
    }
}