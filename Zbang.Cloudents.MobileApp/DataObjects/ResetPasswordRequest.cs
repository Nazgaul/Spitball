using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Zbang.Cloudents.MobileApp.DataObjects
{
    public class ResetPasswordRequest
    {
        [Required]
        public string Email { get; set; }
    }
}