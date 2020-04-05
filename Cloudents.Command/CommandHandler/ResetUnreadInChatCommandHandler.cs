using Cloudents.Command.Command;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.CommandHandler
{
    public class ResetUnreadInChatCommandHandler : ICommandHandler<ResetUnreadInChatCommand>
    {
        private readonly IChatRoomRepository _chatRoomRepository;

        private readonly IRepository<ChatUser> _chatUserRepository;

        public ResetUnreadInChatCommandHandler(IChatRoomRepository chatRoomRepository,
            IRepository<ChatUser> chatUserRepository)
        {
            _chatRoomRepository = chatRoomRepository;
            _chatUserRepository = chatUserRepository;
        }


        public async Task ExecuteAsync(ResetUnreadInChatCommand message, CancellationToken token)
        {
            // var users = message.ToUsersId.ToList();
            //  users.Add(message.UserSendingId);
            var chatRoom = await _chatRoomRepository.LoadAsync(message.Identifier, token);
            //if (chatRoom == null)
            //{
            //    throw new NullReferenceException("no such room");
            //}
            var user = chatRoom.Users.AsQueryable().Single(f => f.User.Id == message.UserSendingId);
            user.ResetUnread();
            await _chatUserRepository.UpdateAsync(user, token);
        }

    }
}