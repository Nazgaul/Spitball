using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Web.Models
{
    public class PaymeSuccessCallback
    {
        [ModelBinder(Name = "payme_status")]
        public string Status { get; set; }

        public long UserId { get; set; }
        //payme_signature
        //payme_sale_id
        //payme_transaction_id
        //price
        //currency
        //transaction_id
        //is_token_sale
        //is_foreign_card
    }
}
