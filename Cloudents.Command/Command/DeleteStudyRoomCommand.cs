using System;

namespace Cloudents.Command.Command
{
    public class DeleteStudyRoomCommand : ICommand
    {
        public DeleteStudyRoomCommand(Guid id, long userId)
        {
            Id = id;
            UserId = userId;
        }

        public Guid Id { get; private set; }
        public long UserId { get; private set; }
    }
}