using System;
using System.Linq;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class ChatAddMessageCommandHandler : ICommandHandler<ChatAddMessageCommand>
    {
        private readonly IRepository<ChatMessage> m_ChatMessageRepository;
        private readonly IRepository<ChatRoom> m_ChatRoomRepository;
        private readonly IChatUserRepository m_ChatUserRepository;
        private readonly IUserRepository m_UserRepository;

        public ChatAddMessageCommandHandler(IRepository<ChatMessage> chatMessageRepository, IRepository<ChatRoom> chatRoomRepository, IUserRepository userRepository, IChatUserRepository chatUserRepository)
        {
            m_ChatMessageRepository = chatMessageRepository;
            m_ChatRoomRepository = chatRoomRepository;
            m_UserRepository = userRepository;
            m_ChatUserRepository = chatUserRepository;
        }

        public void Handle(ChatAddMessageCommand message)
        {
            var chatRoom = GetChatRoom(message);
            message.ChatRoomId = chatRoom.Id;

            var userAction = m_UserRepository.Load(message.UserId);
            message.Message = TextManipulation.EncodeComment(message.Message);
            var chatMessage = new ChatMessage(chatRoom, userAction, message.Message, message.BlobName);

            foreach (var user in chatRoom.Users)
            {
                if (user.User.Id == message.UserId)
                {
                    user.Unread = 0;
                }
                else
                {
                    user.Unread++;
                }
                m_ChatUserRepository.Save(user);
            }

            chatRoom.UpdateTime = DateTime.UtcNow;
            m_ChatRoomRepository.Save(chatRoom);


            m_ChatMessageRepository.Save(chatMessage);
        }


        private ChatRoom GetChatRoom(ChatAddMessageCommand message)
        {
            if (message.ChatRoomId.HasValue)
            {
                return m_ChatRoomRepository.Load(message.ChatRoomId);
            }
            var chatId = m_ChatUserRepository.GetChatRoom(message.UsersInChat);
            if (chatId.HasValue)
            {
                return m_ChatRoomRepository.Load(chatId);
            }
            var chatRoom = new ChatRoom(message.UsersInChat.Select(s => m_UserRepository.Load(s)));
            m_ChatRoomRepository.Save(chatRoom);
            return chatRoom;
        }
    }
}