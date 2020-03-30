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
            ChatRoom chatRoom;
            if (message.Identifier != null)
            {
                chatRoom = await _chatRoomRepository.GetChatRoomAsync(message.Identifier, token);
            }
            else
            {
                var users = new[] { message.ToUsersId, message.UserSendingId };
                chatRoom = await _chatRoomRepository.GetOrAddChatRoomAsync(users, token);
            }

            
            var user = _userRepository.Load(message.UserSendingId);
            chatRoom.AddTextMessage(user, message.Message);
            await _chatRoomRepository.UpdateAsync(chatRoom, token);
        }
    }
}