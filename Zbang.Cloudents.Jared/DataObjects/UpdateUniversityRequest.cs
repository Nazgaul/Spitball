using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Zbang.Cloudents.Jared.DataObjects
{
    public class UpdateUniversityRequest
    {
        [Required]
        public long UniversityId { get; set; }
    }
}