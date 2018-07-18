using System;
using System.Diagnostics.CodeAnalysis;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Command
{
    public class DeleteAnswerCommand : ICommand
    {
        public DeleteAnswerCommand(Guid id, long userId)
        {
            Id = id;
            UserId = userId;
        }

        [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "need for automapper")]
        public DeleteAnswerCommand()
        {
        }

        public Guid Id { get; set; }
        public long UserId { get; set; }
    }
}