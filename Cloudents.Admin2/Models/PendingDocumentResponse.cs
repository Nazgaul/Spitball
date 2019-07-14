using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cloudents.Core.DTOs.Admin;

namespace Cloudents.Admin2.Models
{
    public class PendingDocumentResponse
    {
        public IEnumerable<PendingDocumentDto> Documents { get; set; }

        public string NextLink { get; set; }
    }
}
