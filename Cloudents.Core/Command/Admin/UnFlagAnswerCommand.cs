using System;
using Cloudents.Application.Interfaces;

namespace Cloudents.Application.Command.Admin
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
