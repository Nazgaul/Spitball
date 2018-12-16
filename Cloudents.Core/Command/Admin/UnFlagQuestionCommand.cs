﻿using Cloudents.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cloudents.Core.Command.Admin
{
    public class UnFlagQuestionCommand : ICommand
    {
        public UnFlagQuestionCommand(long questionId)
        {
            QuestionIds = new[] { questionId };
        }

        public UnFlagQuestionCommand(IEnumerable<long> questionIds)
        {
            QuestionIds = questionIds;
        }

        public IEnumerable<long> QuestionIds { get; }
    }
}