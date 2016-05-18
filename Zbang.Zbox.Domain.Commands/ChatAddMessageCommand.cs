using System;
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class ChatAddMessageCommand : ICommand
    {
        public ChatAddMessageCommand(Guid chatRoomId, long userId, string message)
        {
            ChatRoomId = chatRoomId;
            UserId = userId;
            Message = message;
        }

        public Guid ChatRoomId { get; private set; }
        public long UserId { get; private set; }
        public string Message { get; private set; }
    }
}