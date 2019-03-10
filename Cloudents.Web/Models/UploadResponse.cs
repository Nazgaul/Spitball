﻿
using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Cloudents.Web.Models
{
    public class UploadResponse
    {
        public UploadResponse(Guid sessionId)
        {
            Data = new UploadInnerResponse(sessionId);
        }

        public UploadResponse(string fileName)
        {
            FileName = fileName;
        }

        public UploadResponse()
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

    public class UploadRequestFirstStage
    {
        public UploadPhase Phase { get; set; }

        [JsonProperty("mime_type")]
        public string MimeType { get; set; }
        public long Size { get; set; }
        public string Name { get; set; }

        [JsonProperty("session_id")]
        public Guid SessionId { get; set; }
    }

    public class TempData
    {
        public string Name { get; set; }

        public string BlobName { get; set; }

        public double Size { get; set; }
        public string MimeType { get; set; }
        //public IList<long> Indexes { get; set; }
    }

    public enum UploadPhase
    {
        Start,
        Upload,
        Finish
    }

    public class UploadRequest2
    {
        public UploadPhase Phase { get; set; }
        [FromForm(Name="session_id")]
        public Guid SessionId { get; set; }
        [FromForm(Name= "start_offset")]
        public long StartOffset { get; set; }

        [Required]
        public IFormFile Chunk { get; set; }
    }
}