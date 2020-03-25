using Cloudents.Command.Command;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.CommandHandler
{
    public class SendChatTextMessageCommandHandler : ICommandHandler<SendChatTextMessageCommand>
    {
        private readonly IChatRoomRepository _chatRoomRepository;
        private readonly IRegularUserRepository _userRepository;


        public SendChatTextMessageCommandHandler(IChatRoomRepository chatRoomRepository,
            IRegularUserRepository userRepository)
        {
            _chatRoomRepository = chatRoomRepository;
            _userRepository = userRepository;
        }

        public async Task ExecuteAsync(SendChatTextMessageCommand message, CancellationToken token)
        {
            var users = new[] { message.ToUsersId, message.UserSendingId };

            var chatRoom = await _chatRoomRepository.GetChatRoomAsync(users, token);

            if (chatRoom == null)
            {
                var userSending = await _userRepository.LoadAsync(message.UserSendingId, token);
                var userReceiving = await _userRepository.LoadAsync(message.ToUsersId, token);
                chatRoom = new ChatRoom(new List<User>() { userSending, userReceiving });
                await _chatRoomRepository.AddAsync(chatRoom, token);
            }

            var user = _userRepository.Load(message.UserSendingId);

            var chatMessage = new ChatTextMessage(user, message.Message, chatRoom);
            chatRoom.AddMessage(chatMessage);
            await _chatRoomRepository.UpdateAsync(chatRoom, token);
        }
    }
}