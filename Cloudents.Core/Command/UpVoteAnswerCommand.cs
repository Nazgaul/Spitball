using System;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Command
{
    public class UpVoteAnswerCommand : ICommand
    {
        public Guid AnswerId { get; private set; }
    }
}