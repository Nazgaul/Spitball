using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Web.Models
{
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