using System.Collections.Generic;
using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Query
{
    public class QuestionSubjectQuery : IQuery<IEnumerable<QuestionSubjectDto>>
    {
        
    }


    public class EmptyQuery : IQuery<string>
    {

    }
}