using Microsoft.AspNetCore.Http;

namespace Cloudents.Web.Models
{
    public class UploadFileRequest
    {
        public IFormFile File { get; set; }
    }
}
