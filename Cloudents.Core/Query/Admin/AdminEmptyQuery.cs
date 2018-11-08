using System.Collections.Generic;
using Cloudents.Core.DTOs.Admin;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Query.Admin
{
    public class AdminEmptyQuery : 
        IQuery<IEnumerable<QuestionWithoutCorrectAnswerDto>>,
        IQuery<IEnumerable<DTOs.QuestionFeedDto>>,
        IQuery<IEnumerable<CashOutDto>>,
        IQuery<IEnumerable<PendingQuestionDto>>, IQuery<long>
    {
        
    }
}