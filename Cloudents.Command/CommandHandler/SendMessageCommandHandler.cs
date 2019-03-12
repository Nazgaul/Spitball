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
            // IRepository<ChatUser> chatUserRepository,
            IChatDirectoryBlobProvider blobProvider, IRepository<ChatMessage> chatMessageRepository)
        {
            _chatRoomRepository = chatRoomRepository;
            _userRepository = userRepository;
            // _chatMessageRepository = chatMessageRepository;
            //_chatUserRepository = chatUserRepository;
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
                chatRoom = new ChatRoom(users.Select(s => _userRepository.Load(s)));
                await _chatRoomRepository.AddAsync(chatRoom, token);
            }
            var chatMessage = chatRoom.AddMessage(message.UserSendingId, message.Message, message.Blob);
            //var chatUser = chatRoom.Users.FirstOrDefault(f => f.User.Id == message.UserSendingId);
            //var chatMessage = new ChatMessage(chatUser, message.Message,message.Blob);
            //await _chatMessageRepository.AddAsync(chatMessage, token);
            //chatRoom.UpdateTime = DateTime.UtcNow;
            await _chatRoomRepository.UpdateAsync(chatRoom, token);
            await _chatMessageRepository.AddAsync(chatMessage, token); // need this in order to get id from nhibernate
            //var t = Task.CompletedTask;


            if (!string.IsNullOrEmpty(message.Blob))
            {
                var id = chatMessage.Id;
                await _blobProvider.MoveAsync(message.Blob, id.ToString(), token);
            }
        }
    }
}