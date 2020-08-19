using System;
using Newtonsoft.Json;

namespace Cloudents.Admin2.Models
{
    public class UploadRequestFinish : UploadRequestBase
    {
        public UploadPhase Phase { get; set; }

        [JsonProperty("session_id")]
        public Guid SessionId { get; set; }
    }
}