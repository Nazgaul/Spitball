using Newtonsoft.Json;

namespace Cloudents.FunctionsV2.Models
{
    public class Block
    {
        public Block(string title, string subtitle, string body, string minorTitle)
        {
            Title = title;
            Subtitle = subtitle;
            Body = body;
            MinorTitle = minorTitle;
        }

        public Block(string title, string subtitle, string body, string minorTitle, string cta, string url)
        {
            Title = title;
            Subtitle = subtitle;
            Body = body;
            Cta = cta;
            Url = url;
            MinorTitle = minorTitle;
        }

        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("subtitle")]
        public string Subtitle { get; set; }
        [JsonProperty("body")]
        public string Body { get; set; }
        [JsonProperty("cta")]
        public string Cta { get; set; }
        [JsonProperty("url")]
        public string Url { get; set; }
        [JsonProperty("minorTitle")]
        public string MinorTitle { get; set; }
    }
}