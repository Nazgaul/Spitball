﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zbang.Zbox.ViewModel.SqlQueries
{
    public static class Updates
    {
        public const string GetUserUpdates = @"select BoxId,QuestionId,AnswerId,ItemId,QuizId from zbox.NewUpdates
where UserId = @userid";
    }
}
