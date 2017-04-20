using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Zbang.Cloudents.Jared.Models
{
    public class CreateUniversityRequest
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [MaxLength(2)]
        public string Country { get; set; }
    }
}