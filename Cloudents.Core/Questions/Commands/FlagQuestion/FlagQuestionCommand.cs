using Cloudents.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cloudents.Core.Questions.Commands.FlagQuestion
{
    public class FlagQuestionCommand : ICommand
    {
        public FlagQuestionCommand(long userId, long questionId, string flagReason)
        {
            UserId = userId;
            QuestionId = questionId;
            FlagReason = flagReason;
        }

        public long UserId { get; }
        public long QuestionId { get; }
        public string FlagReason { get; }
    }
}
