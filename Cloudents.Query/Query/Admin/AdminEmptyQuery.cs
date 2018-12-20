﻿using System.Collections.Generic;
using Cloudents.Core.DTOs;
using Cloudents.Core.DTOs.Admin;

namespace Cloudents.Query.Query.Admin
{
    public class AdminEmptyQuery :
        IQuery<IEnumerable<QuestionWithoutCorrectAnswerDto>>,
        IQuery<IEnumerable<QuestionFeedDto>>,
        IQuery<IEnumerable<CashOutDto>>,
        IQuery<IEnumerable<PendingQuestionDto>>,
            IQuery<IList<FictivePendingQuestionDto>>,
        IQuery<IEnumerable<SuspendedUsersDto>>,
        IQuery<IList<PendingDocumentDto>>,
        IQuery<IEnumerable<PendingAnswerDto>>,
        IQuery<IEnumerable<FlaggedAnswerDto>>,
        IQuery<IList<FlaggedDocumentDto>>,
        IQuery<IEnumerable<FlaggedQuestionDto>>
    {

    }
}