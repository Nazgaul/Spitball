using System.ComponentModel.DataAnnotations;

namespace Cloudents.Web.Models
{
    public class PayPalTransactionRequest
    {
        [Required]
        public string Id { get; set; }
    }
}