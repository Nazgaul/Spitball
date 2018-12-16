using System;
using System.Collections.Generic;
using System.Text;

namespace Cloudents.Core.Documents.Queries.GetDocumentsList
{
    public class DocumentSearchResultWithScore
    {
        public long Id { get; set; }

        public double Score { get; set; }

        public string MetaContent { get; set; }
    }
}
