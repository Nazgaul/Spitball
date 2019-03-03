using Cloudents.Command.Command;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.CommandHandler
{
    public class ChatAddMessageCommandHandler : ICommandHandler<ChatAddMessageCommand>
    {
        private readonly IChatRoomRepository _chatRoomRepository;
        private readonly IRegularUserRepository _userRepository;
        private readonly IRepository<ChatMessage> _chatMessageRepository;

        public ChatAddMessageCommandHandler(IChatRoomRepository chatRoomRepository, IRegularUserRepository userRepository, IRepository<ChatMessage> chatMessageRepository)
        {
            _chatRoomRepository = chatRoomRepository;
            _userRepository = userRepository;
            _chatMessageRepository = chatMessageRepository;
        }

        public async Task ExecuteAsync(ChatAddMessageCommand message, CancellationToken token)
        {
            var users = message.ToUsersId.ToList();
            users.Add(message.UserSendingId);
            var chatRoom = await _chatRoomRepository.GetChatRoomAsync(users, token);

            if (chatRoom == null)
            {
                chatRoom = new ChatRoom(users.Select(s => _userRepository.Load(s)));
                await _chatRoomRepository.AddAsync(chatRoom, token);
            }

            var chatUser = chatRoom.Users.FirstOrDefault(f => f.User.Id == message.UserSendingId);
            var chatMessage = new ChatMessage(chatUser, message.Message);
            await _chatMessageRepository.AddAsync(chatMessage, token);
            chatRoom.UpdateTime = DateTime.UtcNow;
            await _chatRoomRepository.UpdateAsync(chatRoom, token);
            //var t = Task.CompletedTask;
            //foreach (var user in chatRoom.Users)
            //{
            //    if (message.UserId != user.User.Id && !user.User.Online)
            //    {
            //        t = m_QueueRepository.InsertMessageToMailNewAsync(new MessageMailData(
            //            message.Message,
            //            user.User.Email,
            //            userAction.Name,
            //            userAction.ImageLarge,
            //            userAction.Email,
            //            user.User.Culture,
            //            user.User.Id,
            //            chatRoom.Id, userAction.Id));
            //    }
            //    if (user.User.Id == message.UserId)
            //    {
            //        user.Unread = 0;
            //    }
            //    else
            //    {
            //        user.Unread++;
            //    }
            //    m_ChatUserRepository.Save(user);
            //}


            //m_ChatRoomRepository.Save(chatRoom);

            //m_ChatMessageRepository.Save(chatMessage);
        }
    }
}