using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Cloudents.Web.Models
{
    public class LoginRequest
    {
        [EmailAddress,Required]
        public string Email { get; set; }

        [Required]
        public string Key { get; set; }
    }
}
