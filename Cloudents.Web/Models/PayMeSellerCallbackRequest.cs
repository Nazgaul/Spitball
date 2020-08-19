using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Web.Models
{
    public class PayMeSellerCallbackRequest
    {
        [ModelBinder(Name = "status_code")]
        public int StatusCode { get; set; }

        [ModelBinder(Name = "seller_payme_id")]
        [Required]
        public string SellerKey { get; set; }

        [ModelBinder(Name = "seller_email")]
        [Required]
        public string Email { get; set; }
    }
}