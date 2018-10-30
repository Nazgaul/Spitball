using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cloudents.Core.Enum;

namespace Cloudents.Web.Models
{
    public class CreateDocumentRequest
    {
        public string BlobName { get; set; }
        public string Name { get; set; }
        public DocumentType Type { get; set; }

        public string[] Courses { get; set; }
        public string[] Tags { get; set; }
    }
}
