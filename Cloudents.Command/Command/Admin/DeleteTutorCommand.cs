
using System;

namespace Cloudents.Command.Command.Admin
{
    public class DeleteTutorCommand : ICommand
    {
        public DeleteTutorCommand(long id, Guid userId)
        {
            Id = id;
            UserId = userId;
        }
        public long Id { get; }
        public Guid UserId { get;  }
    }
}
