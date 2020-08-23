using Newtonsoft.Json;

namespace Cloudents.Web.Models
{
    public class UploadRequestStart : UploadRequestBase
    {
        public UploadPhase Phase { get; set; }

        [JsonProperty("mime_type")]
        public string MimeType { get; set; }
        public long Size { get; set; }
        public string Name { get; set; }

        //[JsonProperty("session_id")]
        //public Guid SessionId { get; set; }
    }
}