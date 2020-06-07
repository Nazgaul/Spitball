﻿using System;
using Cloudents.Command.Command;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Storage;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.CommandHandler
{
    public class SendChatFileMessageCommandHandler : ICommandHandler<SendChatFileMessageCommand>
    {
        private readonly IChatRoomRepository _chatRoomRepository;
        private readonly IRegularUserRepository _userRepository;

        private readonly IRepository<ChatMessage> _chatMessageRepository;
        private readonly IChatDirectoryBlobProvider _blobProvider;
        private readonly ITutorRepository _tutorRepository;


        public SendChatFileMessageCommandHandler(
            IChatRoomRepository chatRoomRepository,
            IRegularUserRepository userRepository,
            IRepository<ChatMessage> chatMessageRepository,
            IChatDirectoryBlobProvider blobProvider, ITutorRepository tutorRepository)
        {
            _chatRoomRepository = chatRoomRepository;
            _userRepository = userRepository;
            _blobProvider = blobProvider;
            _tutorRepository = tutorRepository;
            _chatMessageRepository = chatMessageRepository;
        }

        public async Task ExecuteAsync(SendChatFileMessageCommand message, CancellationToken token)
        {

            ChatRoom? chatRoom = null;
            if (!string.IsNullOrEmpty(message.Identifier))
            {
                chatRoom = await _chatRoomRepository.GetChatRoomAsync(message.Identifier, token);
                if (chatRoom == null)
                {
                    var users = ChatRoom.IdentifierToUserIds(message.Identifier);
                    var tutor = await _tutorRepository.LoadAsync(message.TutorId!.Value, token);
                    chatRoom = await _chatRoomRepository.GetOrAddChatRoomAsync(users, tutor, token);
                    //chatRoom = await _chatRoomRepository.GetOrAddChatRoomAsync(users, token);
                }
            }

            //if (chatRoom == null)
            //{
            //    //chatRoom = new ChatRoom(users.Select(s => _userRepository.Load(s)).ToList());
            //    //
            //    //await _chatRoomRepository.AddAsync(chatRoom, token);
            //    var users = message.ToUsersId.ToList();
            //    users.Add(message.UserSendingId);
            //    chatRoom = await _chatRoomRepository.GetOrAddChatRoomAsync(users, token);
            //}
            if (chatRoom == null)
            {
                throw new ArgumentException("Cant create new chat message");
            }


            var user = await _userRepository.LoadAsync(message.UserSendingId, token);
            var chatMessage = new ChatAttachmentMessage(user, message.Blob, chatRoom);
            chatRoom.AddMessage(chatMessage);
            await _chatRoomRepository.UpdateAsync(chatRoom, token);
            await _chatMessageRepository.AddAsync(chatMessage, token); // need this in order to get id from nhibernate
            var id = chatMessage.Id;
            await _blobProvider.MoveAsync(message.Blob, $"{chatRoom.Id}/{id}", token);
        }
    }
}