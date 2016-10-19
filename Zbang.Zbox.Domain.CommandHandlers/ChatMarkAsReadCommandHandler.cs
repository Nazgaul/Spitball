using System;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class ChatMarkAsReadCommandHandler : ICommandHandler<ChatMarkAsReadCommand>
    {
        private readonly IChatUserRepository m_ChatUserRepository;

        public ChatMarkAsReadCommandHandler(IChatUserRepository chatUserRepository)
        {
            m_ChatUserRepository = chatUserRepository;
        }

        public void Handle(ChatMarkAsReadCommand message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            m_ChatUserRepository.DeleteUserUpdateByFeedId(message.UserId, message.RoomId);
        }
    }
}
