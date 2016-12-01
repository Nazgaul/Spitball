﻿using System;
using System.Collections.Generic;
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands.Quiz
{
    public class SaveUserQuizCommand : ICommandAsync
    {
        public SaveUserQuizCommand(IEnumerable<UserAnswers> answers, long userId, long quizId, TimeSpan timeTaken, long boxId)
        {
            BoxId = boxId;
            Answers = answers;
            UserId = userId;
            QuizId = quizId;
            TimeTaken = timeTaken;
        }
        public long UserId { get; private set; }
        public long QuizId { get; private set; }
        public TimeSpan TimeTaken { get; private set; }
        public long BoxId { get; private set; }

        public IEnumerable<UserAnswers> Answers { get; private set; }
    }

    public class UserAnswers
    {
        public Guid QuestionId { get; set; }
        public Guid AnswerId { get; set; }
    }
}
