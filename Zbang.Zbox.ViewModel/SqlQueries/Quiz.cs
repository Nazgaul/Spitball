﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zbang.Zbox.ViewModel.SqlQueries
{
    public static class Quiz
    {
        public const string QuizQuery = @"select q.name, q.id, q.UserId as ownerid,q.BoxId,
(select u.UserName from zbox.Users u where u.userid = q.UserId) as Owner,
q.CreationTime as date,
q.NumberOfViews,
q.Rate,
q.Publish
 from zbox.Quiz q 
where id = @QuizId;";

        public const string Question = @"select q.Id, q.Text,q.RightAnswerId as correctAnswer from zbox.QuizQuestion q where QuizId = @QuizId;";

        public const string Answer = @"select a.id, a.text,a.QuestionId from zbox.QuizAnswer a where QuizId = @QuizId;";

        public const string UserQuiz = @"select q.TimeTaken,q.Score from zbox.SolvedQuiz q where QuizId = @QuizId and UserId = @UserId ;";
        public const string UserAnswer = @"select q.AnswerId,q.QuestionId from zbox.SolvedQuestion q where QuizId = @QuizId and UserId = @UserId;";

        public const string Discussion = @"select qd.Id,
u.UserName,
qd.CreationTime as Date,
u.UserImage as UserPicture,
u.UserId,
qd.Text,
qd.QuestionId
from zbox.QuizDiscussion qd
left join zbox.Users u on u.UserId = qd.UserId
where qd.QuizId = @QuizId";
    }
}
