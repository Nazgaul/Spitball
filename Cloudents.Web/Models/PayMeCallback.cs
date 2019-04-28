using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Cloudents.Web.Models
{
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public class PayMeCallback
    {
        [ModelBinder(Name = "status_code")]
        public int StatusCode { get; set; }

        [ModelBinder(Name = "buyer_key")]
        public string BuyerKey { get; set; }

        [ModelBinder(Name = "buyer_card_exp")]
        public string BuyerCardExp { get; set; }
    }
}
