using System.Collections.Generic;
using Cloudents.Core.DTOs.Admin;

namespace Cloudents.Admin2.Models
{
    public class PendingDocumentResponse
    {
        public IEnumerable<PendingDocumentDto> Documents { get; set; }

        public string NextLink { get; set; }
    }
}
