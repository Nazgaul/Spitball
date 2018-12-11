using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Cloudents.Core.Attributes;
using Cloudents.Core.Enum;
using Cloudents.Core.Extension;

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
