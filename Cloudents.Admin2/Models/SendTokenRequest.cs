using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Cloudents.Admin2.Models
{
    public class SendTokenRequest
    {
        /// <summary>
        /// User id
        /// </summary>
        [Required]
        public long UserId { get; set; }
        /// <summary>
        /// Amount of token that are earned
        /// </summary>
        [Required]
        public decimal Tokens { get; set; }
    }
}
