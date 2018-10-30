using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace Cloudents.Web.Models
{
    public class UploadAskFileRequest
    {
        public IEnumerable<IFormFile> File { get; set; }
    }
}
