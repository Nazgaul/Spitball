﻿using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Command.Admin
{
    public class UnFlagQuestionCommand : ICommand
    {
        public UnFlagQuestionCommand(long questionId)
        {
            QuestionId = questionId ;
        }
        
        public long QuestionId { get; }
    }
}
