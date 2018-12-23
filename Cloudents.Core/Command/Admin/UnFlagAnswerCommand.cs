using Cloudents.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

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
