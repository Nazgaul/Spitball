using System.Linq;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class ChatAddMessageCommandHandler : ICommandHandler<ChatAddMessageCommand>
    {
        //private readonly IDocumentDbRepository<ChatRoom> m_ChatRoomRepository;
        //private readonly IDocumentDbRepository<ChatMessage> m_ChatMessageRepository;
        private readonly IRepository<ChatMessage> m_ChatMessageRepository;
        private readonly IRepository<ChatRoom> m_ChatRoomRepository;
        private readonly IRepository<ChatUser> m_ChatUserRepository;
        private readonly IUserRepository m_UserRepository;

        public ChatAddMessageCommandHandler(IRepository<ChatMessage> chatMessageRepository, IRepository<ChatRoom> chatRoomRepository, IUserRepository userRepository, IRepository<ChatUser> chatUserRepository)
        {
            m_ChatMessageRepository = chatMessageRepository;
            m_ChatRoomRepository = chatRoomRepository;
            m_UserRepository = userRepository;
            m_ChatUserRepository = chatUserRepository;
        }

        //public ChatAddMessageCommandHandler(IDocumentDbRepository<ChatRoom> chatRoomRepository, IDocumentDbRepository<ChatMessage> chatMessageRepository)
        //{
        //    m_ChatRoomRepository = chatRoomRepository;
        //    m_ChatMessageRepository = chatMessageRepository;
        //}

        public void Handle(ChatAddMessageCommand message)
        {
            var chatRoom = m_ChatRoomRepository.Load(message.ChatRoomId);
            var userAction = m_UserRepository.Load(message.UserId);
            var chatMessage = new ChatMessage(chatRoom, userAction, message.Message);

            foreach (var user in chatRoom.Users.Where(w=>w.User.Id != message.UserId))
            {
                user.Unread++;
                m_ChatUserRepository.Save(user);
            }



            m_ChatMessageRepository.Save(chatMessage);
            //var chatRoom = await m_ChatRoomRepository.GetItemAsync(message.ChatRoomId.ToString());
            //if (chatRoom == null)
            //{
            //    throw new ArgumentNullException();
            //}
            //var chatMessage = new ChatMessage(message.ChatRoomId, message.UserId, message.Message);
            //var users = chatRoom.Users.Where(w => w.Id != message.UserId);
            //foreach (var user in users)
            //{
            //    user.UnreadCount++;
            //}
            //var t1 = m_ChatRoomRepository.UpdateItemAsync(message.ChatRoomId.ToString(), chatRoom);
            //var t2 =  m_ChatMessageRepository.CreateItemAsync(chatMessage);
            //await Task.WhenAll(t1, t2);

        }
    }
}