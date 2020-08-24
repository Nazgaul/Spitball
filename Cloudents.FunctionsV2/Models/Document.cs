using Newtonsoft.Json;

namespace Cloudents.FunctionsV2.Models
{
    public class Document
    {
        [JsonProperty("fileUrl")]
        public string Url { get; set; }
        [JsonProperty("fileName")]
        public string Name { get; set; }
        [JsonProperty("uploader")]
        public string UserName { get; set; }
        [JsonProperty("imgSource")]
        public string DocumentPreview { get; set; }

        [JsonProperty("uploaderImage")] public string UserImage { get; set; }
    }
}