using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Cloudents.Web.Models
{
    public class UploadFileRequest
    {
        public IFormFile File { get; set; }
    }
}
