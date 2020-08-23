using System;

namespace Cloudents.Admin2.Models
{
    public class UploadEndResponce : UploadStartResponse
    {
        public UploadEndResponce(Uri url)
        {
            Url = url;
        }
        public Uri Url { get; set; }
    }
}