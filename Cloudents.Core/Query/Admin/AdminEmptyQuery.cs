using System.Collections.Generic;
using Cloudents.Core.DTOs.Admin;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Query.Admin
{
    public class AdminEmptyQuery : 
        IQuery<IEnumerable<QuestionWithoutCorrectAnswerDto>>,
        IQuery<IEnumerable<Cloudents.Core.DTOs.QuestionDto>>,
        IQuery<IEnumerable<CashOutDto>>, IQuery<long>
    {
        
    }
}