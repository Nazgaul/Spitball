using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class ChatCreateRoomCommandHandler : ICommandHandlerAsync<ChatCreateRoomCommand>
    {
        private readonly IDocumentDbRepository<ChatRoom> m_ChatRoomRepository;

        public ChatCreateRoomCommandHandler(IDocumentDbRepository<ChatRoom> chatRoomRepository)
        {
            m_ChatRoomRepository = chatRoomRepository;
        }

        public Task HandleAsync(ChatCreateRoomCommand message)
        {
            var chatRoom = new ChatRoom(message.Id, message.UserIds.Select(s => new ChatUser(s)));
            return m_ChatRoomRepository.CreateItemAsync(chatRoom);
        }
    }

    public class ChatAddMessageCommandHandler : ICommandHandlerAsync<ChatAddMessageCommand>
    {
        private readonly IDocumentDbRepository<ChatRoom> m_ChatRoomRepository;
        private readonly IDocumentDbRepository<ChatMessage> m_ChatMessageRepository;

        public ChatAddMessageCommandHandler(IDocumentDbRepository<ChatRoom> chatRoomRepository, IDocumentDbRepository<ChatMessage> chatMessageRepository)
        {
            m_ChatRoomRepository = chatRoomRepository;
            m_ChatMessageRepository = chatMessageRepository;
        }

        public async Task HandleAsync(ChatAddMessageCommand message)
        {
            var chatRoom = await m_ChatRoomRepository.GetItemAsync(message.ChatRoomId.ToString());
            if (chatRoom == null)
            {
                throw new ArgumentNullException();
            }
            var chatMessage = new ChatMessage(message.ChatRoomId, message.UserId, message.Message);
            var users = chatRoom.Users.Where(w => w.Id != message.UserId);
            foreach (var user in users)
            {
                user.UnreadCount++;
            }
            var t1 = m_ChatRoomRepository.UpdateItemAsync(message.ChatRoomId.ToString(), chatRoom);
            var t2 =  m_ChatMessageRepository.CreateItemAsync(chatMessage);

            await Task.WhenAll(t1, t2);

        }
    }
}
