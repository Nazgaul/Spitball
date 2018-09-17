using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Cloudents.Admin2.Models
{
    public class SendTokenRequest
    {
        [Required]
        public long UserId { get; set; }
        [Required]
        public decimal Tokens { get; set; }
    }
}
