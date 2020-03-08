using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Cloudents.Web.Models
{
    public class PayPalOrderRequest
    {
        [Required]
        public string OrderId { get; set; }
    }
}
