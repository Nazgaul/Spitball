using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cloudents.Web.Models
{
    public class SimilarDocumentsRequest
    {
        public string Course { get; set; }
        public long DocumentId { get; set; }
    }
}
