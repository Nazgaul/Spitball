using Cloudents.Command.Command;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.CommandHandler
{
    public class SendChatTextMessageCommandHandler : ICommandHandler<SendChatTextMessageCommand>
    {
        private readonly IChatRoomRepository _chatRoomRepository;
        private readonly IRegularUserRepository _userRepository;

        private readonly IRepository<ChatMessage> _chatMessageRepository;

        public SendChatTextMessageCommandHandler(IChatRoomRepository chatRoomRepository,
            IRegularUserRepository userRepository,
             IRepository<ChatMessage> chatMessageRepository)
        {
            _chatRoomRepository = chatRoomRepository;
            _userRepository = userRepository;
            _chatMessageRepository = chatMessageRepository;
        }

        public async Task ExecuteAsync(SendChatTextMessageCommand message, CancellationToken token)
        {
            var users = message.ToUsersId.ToList();
            users.Add(message.UserSendingId);
            var chatRoom = await _chatRoomRepository.GetChatRoomAsync(users, token);

            if (chatRoom == null)
            {
                chatRoom = new ChatRoom(users.Select(s => _userRepository.Load(s)).ToList());
                await _chatRoomRepository.AddAsync(chatRoom, token);
            }

            var user = _userRepository.Load(message.UserSendingId);

            var chatMessage = new ChatTextMessage(user, message.Message, chatRoom);
            chatRoom.AddMessage(chatMessage);
            await _chatRoomRepository.UpdateAsync(chatRoom, token);
            await _chatMessageRepository.AddAsync(chatMessage, token); // need this in order to get id from nhibernate



        }
    }
}