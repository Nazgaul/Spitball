using System;
using Cloudents.Command.Command;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.CommandHandler
{
    public class SendChatTextMessageCommandHandler : ICommandHandler<SendChatTextMessageCommand>
    {
        private readonly IChatRoomRepository _chatRoomRepository;
        private readonly IRegularUserRepository _userRepository;
        private readonly ITutorRepository _tutorRepository;


        public SendChatTextMessageCommandHandler(IChatRoomRepository chatRoomRepository,
            IRegularUserRepository userRepository, ITutorRepository tutorRepository)
        {
            _chatRoomRepository = chatRoomRepository;
            _userRepository = userRepository;
            _tutorRepository = tutorRepository;
        }

        public async Task ExecuteAsync(SendChatTextMessageCommand message, CancellationToken token)
        {
            ChatRoom? chatRoom = null;
            if (message.Identifier != null)
            {
                chatRoom = await _chatRoomRepository.GetChatRoomAsync(message.Identifier, token);

                if (chatRoom == null)
                {
                    var users = ChatRoom.IdentifierToUserIds(message.Identifier);
                    var tutor = await _tutorRepository.LoadAsync(message.TutorId!.Value, token);
                    chatRoom = await _chatRoomRepository.GetOrAddChatRoomAsync(users, tutor, token);
                }

            }
            //if (chatRoom == null && message.ToUsersId.HasValue)
            //{
            //    var users2 = new[] { message.ToUsersId.Value, message.UserSendingId };
            //    chatRoom = await _chatRoomRepository.GetOrAddChatRoomAsync(users2, token);
            //}
            if (chatRoom == null)
            {
                throw new ArgumentException("Cant create new chat message");
            }


            var user = await _userRepository.LoadAsync(message.UserSendingId, token);
            chatRoom.AddTextMessage(user, message.Message);
            await _chatRoomRepository.UpdateAsync(chatRoom, token);
        }
    }
}