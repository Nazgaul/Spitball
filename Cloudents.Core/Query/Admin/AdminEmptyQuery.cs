using Cloudents.Core.DTOs.Admin;
using Cloudents.Core.Interfaces;
using System.Collections.Generic;

namespace Cloudents.Core.Query.Admin
{
    public class AdminEmptyQuery :
        IQuery<IEnumerable<QuestionWithoutCorrectAnswerDto>>,
        IQuery<IEnumerable<DTOs.QuestionFeedDto>>,
        IQuery<IEnumerable<CashOutDto>>,
        IQuery<IEnumerable<PendingQuestionDto>>,
            IQuery<IList<FictivePendingQuestionDto>>,
        IQuery<IEnumerable<SuspendedUsersDto>>,
        IQuery<IList<PendingDocumentDto>>,
        IQuery<IEnumerable<PendingAnswerDto>>

    {

    }
}