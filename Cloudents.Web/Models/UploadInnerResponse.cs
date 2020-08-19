using System;
using Newtonsoft.Json;

namespace Cloudents.Web.Models
{
    public class UploadInnerResponse
    {
        internal const double BlockSize = 3.5e+6;

        public UploadInnerResponse(Guid sessionId)
        {
            SessionId = sessionId;
        }

        [JsonProperty("session_id")]
        public Guid SessionId { get; set; }

        [JsonProperty("end_offset")]
        public double EndOffset => BlockSize;
    }
}