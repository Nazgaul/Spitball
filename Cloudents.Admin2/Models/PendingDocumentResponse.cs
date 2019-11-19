using Cloudents.Core.DTOs.Admin;
using System.Collections.Generic;

namespace Cloudents.Admin2.Models
{
    public class PendingDocumentResponse
    {
        public IEnumerable<PendingDocumentDto> Documents { get; set; }

        public string NextLink { get; set; }
    }
}
