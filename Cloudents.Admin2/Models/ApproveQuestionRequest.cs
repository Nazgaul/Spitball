using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cloudents.Admin2.Models
{
    public class ApproveQuestionRequest
    {
        public long id { get; set; }
    }

    public class ApproveDocumentRequest
    {
        public IEnumerable<long> id { get; set; }
    }


    public class ApproveAnswerRequest
    {
        public IEnumerable<Guid> ids { get; set; }
    }
}
