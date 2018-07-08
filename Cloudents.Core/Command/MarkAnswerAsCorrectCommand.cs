﻿using System;
using System.Diagnostics.CodeAnalysis;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Command
{
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local", Justification = "Automapper handle that")]
    public class MarkAnswerAsCorrectCommand : ICommand
    {
        public MarkAnswerAsCorrectCommand()
        {
            
        }

        public MarkAnswerAsCorrectCommand(Guid answerId, long userId)
        {
            AnswerId = answerId;
            UserId = userId;
        }

        public Guid AnswerId { get; private set; }
        public long UserId { get; private set; }
    }
}