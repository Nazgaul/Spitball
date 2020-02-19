using System;

namespace Cloudents.Command.Command.Admin
{
    public class UnFlagAnswerCommand : ICommand
    {
        public UnFlagAnswerCommand(Guid answerId)
        {
            AnswerId = answerId;
        }

        public Guid AnswerId { get; }
    }
}
