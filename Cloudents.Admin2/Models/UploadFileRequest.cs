using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace Cloudents.Admin2.Models
{
    public class UploadAskFileRequest
    {
        public IEnumerable<IFormFile> File { get; set; }
    }
}
