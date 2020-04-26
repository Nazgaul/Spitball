using System;

namespace Cloudents.Command.Command.Admin
{
    public class DeleteUserCommand : ICommand
    {
        public DeleteUserCommand(long id, Guid userId)
        {
            Id = id;
            UserId = userId;
        }

        public long Id { get; }

        public Guid UserId { get;  }
    }
}