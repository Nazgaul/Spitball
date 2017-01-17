using System;
using System.Linq;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Repositories;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Transport;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class ChatAddMessageCommandHandler : ICommandHandlerAsync<ChatAddMessageCommand>
    {
        private readonly IRepository<ChatMessage> m_ChatMessageRepository;
        private readonly IRepository<ChatRoom> m_ChatRoomRepository;
        private readonly IChatUserRepository m_ChatUserRepository;
        private readonly IUserRepository m_UserRepository;
        private readonly IQueueProvider m_QueueRepository;

        public ChatAddMessageCommandHandler(IRepository<ChatMessage> chatMessageRepository, IRepository<ChatRoom> chatRoomRepository, IUserRepository userRepository, IChatUserRepository chatUserRepository, IQueueProvider queueRepository)
        {
            m_ChatMessageRepository = chatMessageRepository;
            m_ChatRoomRepository = chatRoomRepository;
            m_UserRepository = userRepository;
            m_ChatUserRepository = chatUserRepository;
            m_QueueRepository = queueRepository;
        }

        public Task HandleAsync(ChatAddMessageCommand message)
        {
            var chatRoom = GetChatRoom(message);
            message.ChatRoomId = chatRoom.Id;

            var userAction = m_UserRepository.Load(message.UserId);
            var chatMessage = new ChatMessage(chatRoom, userAction, message.Message, message.BlobName);

            var t = Task.CompletedTask;
            foreach (var user in chatRoom.Users)
            {
                if (!user.User.Online)
                {
                    t = m_QueueRepository.InsertMessageToMailNewAsync(new MessageMailData(
                        message.Message,
                        user.User.Email,
                        userAction.Name,
                        userAction.ImageLarge,
                        userAction.Email,
                        user.User.Culture,
                        user.User.Id,
                        chatRoom.Id));
                }
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
            return t;
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