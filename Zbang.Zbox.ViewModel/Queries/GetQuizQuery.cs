﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zbang.Zbox.ViewModel.Queries
{
    public class GetQuizQuery :QueryBase
    {
        public GetQuizQuery(long quizId, long userId, long boxId)
            :base (userId)
        {
            QuizId = quizId;
            BoxId = boxId;
        }
        public long QuizId { get; private set; }
        public long BoxId { get; private set; }
    }

    public class GetQuizDraftQuery
    {
        public GetQuizDraftQuery(long quizId)
        {
            QuizId = quizId;
        }
        public long QuizId { get;private set; }
    }
}
