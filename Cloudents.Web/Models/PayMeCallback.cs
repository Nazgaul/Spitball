using Microsoft.AspNetCore.Mvc;

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
    }

    public class PayMeSellerCallbackRequest
    {
        [ModelBinder(Name = "status_code")]
        public int StatusCode { get; set; }

        [ModelBinder(Name = "seller_payme_id")]
        public string SellerKey { get; set; }

        [ModelBinder(Name = "seller_email")]
        public string Email { get; set; }
    }
}
