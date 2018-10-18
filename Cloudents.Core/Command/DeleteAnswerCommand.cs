using System;
using System.Diagnostics.CodeAnalysis;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Command
{
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "need for automapper")]
    public class DeleteAnswerCommand : ICommand
    {
        public DeleteAnswerCommand(Guid id, long userId)
        {
            Id = id;
            UserId = userId;
        }

        public Guid Id { get; set; }
        public long UserId { get; set; }
    }
}