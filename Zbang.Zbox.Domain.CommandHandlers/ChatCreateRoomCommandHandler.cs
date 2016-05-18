using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class ChatCreateRoomCommandHandler : ICommandHandler<ChatCreateRoomCommand>
    {
        private readonly IRepository<ChatRoom> m_ChatRoomRepository;
        private readonly IUserRepository m_UserRepository;

        public ChatCreateRoomCommandHandler(IRepository<ChatRoom> chatRoomRepository, IUserRepository userRepository)
        {
            m_ChatRoomRepository = chatRoomRepository;
            m_UserRepository = userRepository;
        }


        public void Handle(ChatCreateRoomCommand message)
        {
            var chatRoom = new ChatRoom(message.Id, message.UserIds.Select(s => m_UserRepository.Load(s)));
            m_ChatRoomRepository.Save(chatRoom);
        }
    }
}
