using Cloudents.Command.Command;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.CommandHandler
{
    public class SendMessageCommandHandler : ICommandHandler<SendMessageCommand>
    {
        private readonly IChatRoomRepository _chatRoomRepository;
        private readonly IRegularUserRepository _userRepository;
        private readonly IRepository<ChatUser> _chatUserRepository;
        private readonly IRepository<ChatMessage> _chatMessageRepository;

        public SendMessageCommandHandler(IChatRoomRepository chatRoomRepository, IRegularUserRepository userRepository, IRepository<ChatMessage> chatMessageRepository, IRepository<ChatUser> chatUserRepository)
        {
            _chatRoomRepository = chatRoomRepository;
            _userRepository = userRepository;
            _chatMessageRepository = chatMessageRepository;
            _chatUserRepository = chatUserRepository;
        }

        public async Task ExecuteAsync(SendMessageCommand message, CancellationToken token)
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
            foreach (var user in chatRoom.Users)
            {
                if (message.UserSendingId != user.User.Id && !user.User.Online)
                {
                   //TODO: need to send an email or something
                }
                if (user.User.Id == message.UserSendingId)
                {
                    user.Unread = 0;
                }
                else
                {
                    user.Unread++;
                }
                await _chatUserRepository.UpdateAsync(user, token);
            }
        }
    }
}