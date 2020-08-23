using System;

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
}