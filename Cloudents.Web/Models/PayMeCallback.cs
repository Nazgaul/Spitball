using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Cloudents.Web.Models
{
    public class PayMeBuyerCallbackRequest
    {
        [ModelBinder(Name = "status_code")]
        public int StatusCode { get; set; }

        [ModelBinder(Name = "buyer_key")]
        public string BuyerKey { get; set; }

        [ModelBinder(Name = "buyer_card_exp")]
        public string BuyerCardExp { get; set; }

        [ModelBinder(Name = "buyer_card_mask")]
        public string BuyerCardMask { get; set; }
    }

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
