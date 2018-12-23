using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cloudents.Admin2.Models
{
    public class UnFlagQuestionRequest
    {
        public long id { get; set; }
    }

    public class UnFlagDocumentRequest
    {
        public IEnumerable<long> id { get; set; }
    }

    public class UnFlagAnswerRequest
    {
        public Guid id { get; set; }
    }
}
