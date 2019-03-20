using Cloudents.Command.Command;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Storage;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.CommandHandler
{
    public class SendMessageCommandHandler : ICommandHandler<SendMessageCommand>
    {
        private readonly IChatRoomRepository _chatRoomRepository;
        private readonly IRegularUserRepository _userRepository;

        //private readonly IRepository<ChatUser> _chatUserRepository;
        private readonly IRepository<ChatMessage> _chatMessageRepository;
        private readonly IChatDirectoryBlobProvider _blobProvider;

        public SendMessageCommandHandler(IChatRoomRepository chatRoomRepository,
            IRegularUserRepository userRepository,
            IChatDirectoryBlobProvider blobProvider, IRepository<ChatMessage> chatMessageRepository)
        {
            _chatRoomRepository = chatRoomRepository;
            _userRepository = userRepository;
            _blobProvider = blobProvider;
            _chatMessageRepository = chatMessageRepository;
        }

        public async Task ExecuteAsync(SendMessageCommand message, CancellationToken token)
        {
            var users = message.ToUsersId.ToList();
            users.Add(message.UserSendingId);
            ChatRoom chatRoom;
            if (message.ChatRoomId.HasValue)
            {
                chatRoom = await _chatRoomRepository.LoadAsync(message.ChatRoomId.Value, token);
            }
            else
            {
                
                chatRoom = await _chatRoomRepository.GetChatRoomAsync(users, token);
            }

            if (chatRoom == null)
            {
                chatRoom = new ChatRoom(users.Select(s => _userRepository.Load(s)).ToList());
                await _chatRoomRepository.AddAsync(chatRoom, token);
            }

            var user = _userRepository.Load(message.UserSendingId);

            var chatMessage = chatRoom.AddMessage(user, message.Message, message.Blob);
            await _chatRoomRepository.UpdateAsync(chatRoom, token);
            await _chatMessageRepository.AddAsync(chatMessage, token); // need this in order to get id from nhibernate


            if (!string.IsNullOrEmpty(message.Blob))
            {
                var id = chatMessage.Id;
                await _blobProvider.MoveAsync(message.Blob, id.ToString(), token);
            }
        }
    }
}