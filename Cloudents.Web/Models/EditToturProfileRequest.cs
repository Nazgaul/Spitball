using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Cloudents.Web.Models
{
    public class EditToturProfileRequest
    {
        [StringLength(255, ErrorMessage = "StringLength")]
        public string Name { get; set; }
        [StringLength(255, ErrorMessage = "StringLength")]
        public string LastName { get; set; }
        [StringLength(1000, ErrorMessage = "StringLength")]
        public string Bio { get; set; }
        [StringLength(255, ErrorMessage = "StringLength")]
        public string Description { get; set; }
    }
}
