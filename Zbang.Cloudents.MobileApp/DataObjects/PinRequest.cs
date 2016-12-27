using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Zbang.Cloudents.MobileApp.DataObjects
{
    public class PinRequest
    {
        [Required]
        [Range(0,int.MaxValue)]
        public int Index { get; set; }
    }
}