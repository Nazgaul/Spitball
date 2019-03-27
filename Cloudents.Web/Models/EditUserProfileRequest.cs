using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Cloudents.Web.Models
{
    public class EditUserProfileRequest
    {
        [StringLength(255, ErrorMessage = "StringLength")]
        public string Name { get; set; }
        [StringLength(255, ErrorMessage = "StringLength")]
        public string Description { get; set; }
    }
}
