using System;
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

        public Guid Id { get; set; }
        public long UserId { get; set; }
    }
}