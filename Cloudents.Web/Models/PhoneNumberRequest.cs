using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Cloudents.Web.Models
{
    public class PhoneNumberRequest
    {
        [Required]
        public string Number { get; set; }
    }


    public class CodeRequest
    {
        [Required]
        public string Number { get; set; }
    }
}
