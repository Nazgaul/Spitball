using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace Cloudents.Admin2.Models
{
    public class UploadStartResponse
    {
        public UploadStartResponse(Guid sessionId)
        {
            Data = new UploadInnerResponse(sessionId);
        }

        public UploadStartResponse(string fileName)
        {
            FileName = fileName;
        }

        public UploadStartResponse()
        {
        }

        public string Status => "success";

        public UploadInnerResponse Data { get; set; }

        public string FileName { get; set; }
    }
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

    public class UploadRequestStart
    {
        public UploadPhase Phase { get; set; }

        [JsonProperty("mime_type")]
        public string MimeType { get; set; }
        public long Size { get; set; }
        public string Name { get; set; }
    }

    public class UploadRequestFinish
    {
        public UploadPhase Phase { get; set; }

        [JsonProperty("session_id")]
        public Guid SessionId { get; set; }
    }
    public enum UploadPhase
    {
        Start,
        Upload,
        Finish
    }

    public class TempData
    {
        public string Name { get; set; }

        public string BlobName { get; set; }

        public double Size { get; set; }
        public string MimeType { get; set; }
        //public IList<long> Indexes { get; set; }
    }

    public class UploadRequestForm
    {
        //public UploadPhase Phase { get; set; }
        [FromForm(Name = "session_id")]
        public Guid SessionId { get; set; }
        [FromForm(Name = "start_offset")]
        public long StartOffset { get; set; }

        [Required]
        public IFormFile Chunk { get; set; }
    }
}
