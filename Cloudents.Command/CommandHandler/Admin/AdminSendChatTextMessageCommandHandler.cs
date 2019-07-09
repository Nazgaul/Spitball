using Cloudents.Command.Command.Admin;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.CommandHandler.Admin
{
    public class AdminSendChatTextMessageCommandHandler : ICommandHandler<AdminSendChatTextMessageCommand>
    {
        private readonly IChatRoomRepository _chatRoomRepository;
        private readonly IRegularUserRepository _userRepository;
        private readonly IRepository<ChatMessage> _chatMessageRepository;

        public AdminSendChatTextMessageCommandHandler(IChatRoomRepository chatRoomRepository,
            IRegularUserRepository userRepository,
             IRepository<ChatMessage> chatMessageRepository)
        {
            _chatRoomRepository = chatRoomRepository;
            _userRepository = userRepository;
            _chatMessageRepository = chatMessageRepository;
        }

        public async Task ExecuteAsync(AdminSendChatTextMessageCommand message, CancellationToken token)
        {
            var users = new[] { message.ToUsersId, message.UserSendingId };

            var chatRoom = await _chatRoomRepository.GetChatRoomAsync(users, token);

            if (chatRoom == null)
            {
                var userSending = await _userRepository.LoadAsync(message.UserSendingId, token);
                var userReceiving = await _userRepository.LoadAsync(message.ToUsersId, token);
                if (userReceiving.Tutor == null)
                {
                    throw new ArgumentException("sending a message not to tutor");
                }
                chatRoom = new ChatRoom(new List<User>() { userSending, userReceiving });
                await _chatRoomRepository.AddAsync(chatRoom, token);
            }

            var user = _userRepository.Load(message.UserSendingId);

            var chatMessage = new SystemTextMessage(user, message.Message, chatRoom);
            chatRoom.AddMessage(chatMessage);
            await _chatRoomRepository.UpdateAsync(chatRoom, token);
            await _chatMessageRepository.AddAsync(chatMessage, token); // need this in order to get id from nhibernate
        }
    }
}
