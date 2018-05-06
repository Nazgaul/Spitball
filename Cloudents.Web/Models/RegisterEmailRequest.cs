using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Cloudents.Web.Models
{
    public class RegisterEmailRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
