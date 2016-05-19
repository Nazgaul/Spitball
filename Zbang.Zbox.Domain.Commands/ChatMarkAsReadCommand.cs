using System;
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class ChatMarkAsReadCommand : ICommand
    {
        public ChatMarkAsReadCommand(long userId, Guid roomId)
        {
            UserId = userId;
            RoomId = roomId;
        }

        public long UserId { get; private set; }
        public Guid RoomId { get; private set; }
    }
}
