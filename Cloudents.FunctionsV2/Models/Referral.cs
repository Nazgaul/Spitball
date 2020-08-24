using Newtonsoft.Json;

namespace Cloudents.FunctionsV2.Models
{
    public class Referral
    {
        public Referral(string link)
        {
            Link = link;
        }


        [JsonProperty("link")]
        public string Link { get; set; }
    }
}