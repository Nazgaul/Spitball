using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Zbang.Cloudents.Mvc4WebRole.Models
{
    public class RateModel
    {
        [Required]
        public long ItemId { get; set; }
        [Required]
        public int Rate { get; set; }
    }
}