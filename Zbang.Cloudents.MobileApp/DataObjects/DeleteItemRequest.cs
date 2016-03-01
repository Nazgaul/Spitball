using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Zbang.Cloudents.MobileApp.DataObjects
{
    public class DeleteItemRequest
    {
        [Required]
        public long BoxId { get; set; }
         [Required]
        public long ItemId { get; set; }
    }
}