using System;
using System.Diagnostics.CodeAnalysis;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Command
{
    public class MarkAnswerAsCorrectCommand : ICommand
    {
        public MarkAnswerAsCorrectCommand(Guid answerId, long userId, string link)
        {
            AnswerId = answerId;
            UserId = userId;
            Link = link;
        }

        public Guid AnswerId { get; private set; }
        public long UserId { get; private set; }

        public string Link { get; private set; }
    }
}