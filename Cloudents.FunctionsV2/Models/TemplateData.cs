using System.Collections.Generic;
using Newtonsoft.Json;

namespace Cloudents.FunctionsV2.Models
{
    public class TemplateData
    {
        [JsonProperty("blocks")]
        public IEnumerable<Block> Blocks { get; set; }
        [JsonProperty("referral")]
        public Referral Referral { get; set; }

        [JsonProperty("subject")]
        public string Subject { get; set; }

        [JsonProperty("to")]
        public string To { get; set; }

    }
}