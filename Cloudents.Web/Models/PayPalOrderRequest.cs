using System.ComponentModel.DataAnnotations;

namespace Cloudents.Web.Models
{
    public class PayPalOrderRequest
    {
        [Required]
        public string OrderId { get; set; }
    }
}
