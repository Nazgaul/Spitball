using System;
using System.Collections.Generic;

namespace Cloudents.Admin2.Models
{
    public class ApproveQuestionRequest
    {
        public IEnumerable<long> Id { get; set; }
    }

    public class ApproveDocumentRequest
    {
        public IEnumerable<long> Id { get; set; }
    }


    //public class ApproveAnswerRequest
    //{
    //    public IEnumerable<Guid> ids { get; set; }
    //}
}
