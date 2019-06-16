using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Command.Command;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;

namespace Cloudents.Command.CommandHandler
{
    public class SendTutorRequestChatTextMessageCommandHandler : ICommandHandler<SendTutorRequestChatTextMessageCommand>
    {
        private readonly ITutorRepository _tutorRepository;
        private readonly IChatRoomRepository _chatRoomRepository;
        private readonly IRegularUserRepository _userRepository;
        private readonly IRepository<ChatMessage> _chatMessageRepository;

        public SendTutorRequestChatTextMessageCommandHandler(ITutorRepository tutorRepository, IChatRoomRepository chatRoomRepository, IRegularUserRepository userRepository, IRepository<ChatMessage> chatMessageRepository)
        {
            _tutorRepository = tutorRepository;
            _chatRoomRepository = chatRoomRepository;
            _userRepository = userRepository;
            _chatMessageRepository = chatMessageRepository;
        }

        public async Task ExecuteAsync(SendTutorRequestChatTextMessageCommand message, CancellationToken token)
        {
            var usersIds = await _tutorRepository.GetTutorsByCourseAsync(message.Course, message.UserId, token);
            var user = await _userRepository.LoadAsync(message.UserId, token);
            foreach (var userId in usersIds)
            {
                var users = new[] {userId, message.UserId};
                var chatRoom = await _chatRoomRepository.GetOrAddChatRoomAsync(users, token);

                var chatMessage = new ChatTextMessage(user, message.Text, chatRoom);
                chatRoom.AddMessage(chatMessage);
                await _chatRoomRepository.UpdateAsync(chatRoom, token);
                await _chatMessageRepository.AddAsync(chatMessage, token);

            }
        }
    }
}