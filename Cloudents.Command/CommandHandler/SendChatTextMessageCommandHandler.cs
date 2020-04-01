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

            var chatRoom = await _chatRoomRepository.GetOrAddChatRoomAsync(users, token);
            var user = _userRepository.Load(message.UserSendingId);
            //var chatMessage = new ChatTextMessage(user, message.Message, chatRoom);
            chatRoom.AddTextMessage(user, message.Message);
            await _chatRoomRepository.UpdateAsync(chatRoom, token);
        }
    }
}