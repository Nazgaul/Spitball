using System;
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class MarkAsAnswerCommand: ICommand
    {
        public MarkAsAnswerCommand(Guid answerId, long userId)
        {
            AnswerId = answerId;
            UserId = userId;
        }

        public Guid AnswerId { get;private set; }

        public long UserId { get; set; }
    }
}
