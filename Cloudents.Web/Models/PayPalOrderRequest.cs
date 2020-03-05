using System;
using System.ComponentModel.DataAnnotations;

namespace Cloudents.Web.Models
{
    public class PayPalOrderRequest
    {
        [Required]
        public string OrderId { get; set; }
        [Required]
        public Guid RoomId { get; set; }
    }
}
