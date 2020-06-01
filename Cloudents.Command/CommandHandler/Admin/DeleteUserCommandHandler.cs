using System;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Command.Command.Admin;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;

namespace Cloudents.Command.CommandHandler.Admin
{
    public class DeleteUserCommandHandler : ICommandHandler<DeleteUserCommand>
    {
        private readonly IRegularUserRepository _userRepository;
        private readonly IRepository<AdminUser> _adminUserRepository;
        private readonly IRepository<ChatRoom> _chatRoomRepository;


        public DeleteUserCommandHandler(IRegularUserRepository userRepository, IRepository<AdminUser> adminUserRepository, IRepository<ChatRoom> chatRoomRepository)
        {
            _userRepository = userRepository;
            _adminUserRepository = adminUserRepository;
            _chatRoomRepository = chatRoomRepository;
        }

        public async Task ExecuteAsync(DeleteUserCommand message, CancellationToken token)
        {
            var user = await _userRepository.LoadAsync(message.Id, token);
            var adminUser = await _adminUserRepository.LoadAsync(message.UserId, token);
            if (adminUser.SbCountry != null && user.SbCountry != adminUser.SbCountry)
            {
                throw new ArgumentException();
            }

            foreach (var chatUser in user.ChatUsers)
            {
                if (chatUser.ChatRoom.StudyRoom == null)
                {
                    await _chatRoomRepository.DeleteAsync(chatUser.ChatRoom, token);
                }
            }

            await _userRepository.DeleteAsync(user, token);
        }
    }
}