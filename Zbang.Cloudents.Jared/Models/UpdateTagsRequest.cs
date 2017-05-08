using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Zbang.Cloudents.Jared.Models
{
    public class UpdateTagsRequest
    {
        [Required]
        public string[] Tags { get; set; }  
    }
}