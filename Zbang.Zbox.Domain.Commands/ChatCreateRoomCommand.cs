using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class ChatCreateRoomCommand : ICommandAsync
    {
        public ChatCreateRoomCommand(IEnumerable<long> userIds, Guid id)
        {
            UserIds = userIds;
            Id = id;
        }

        public IEnumerable<long> UserIds { get; private set; }

        public Guid Id { get; private set; }
    }

    public class ChatAddMessageCommand : ICommandAsync
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
