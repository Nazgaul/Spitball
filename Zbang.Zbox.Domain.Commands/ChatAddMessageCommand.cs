using System;
using System.Collections.Generic;
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class ChatAddMessageCommand : ICommand
    {
        public ChatAddMessageCommand(Guid? chatRoomId, long userId, string message, IEnumerable<long> usersInChat)
        {
            ChatRoomId = chatRoomId;
            UserId = userId;
            Message = message;
            UsersInChat = usersInChat;
        }

        public Guid? ChatRoomId { get; set; }
        public long UserId { get; private set; }
        public string Message { get; set; }

        public IEnumerable<long> UsersInChat { get; private set; }
    }
}