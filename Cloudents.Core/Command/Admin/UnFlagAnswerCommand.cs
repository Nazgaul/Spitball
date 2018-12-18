using Cloudents.Core.Interfaces;
using System;

namespace Cloudents.Core.Command.Admin
{
    public class UnFlagAnswerCommand : ICommand
    {
        public UnFlagAnswerCommand(Guid answerId)
        {
            AnswerId =  answerId;
        }
        
        public Guid AnswerId { get; }
    }
}
