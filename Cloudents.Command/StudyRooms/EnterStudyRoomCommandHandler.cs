﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;

namespace Cloudents.Command.StudyRooms
{
    public class EnterStudyRoomCommandHandler : ICommandHandler<EnterStudyRoomCommand>
    {
        private readonly IRepository<StudyRoom> _studyRoomRepository;
        private readonly IRegularUserRepository _userRepository;
        private readonly IStudyRoomProvider _videoProvider;

        public EnterStudyRoomCommandHandler(IRepository<StudyRoom> studyRoomRepository, IRegularUserRepository userRepository, IStudyRoomProvider videoProvider)
        {
            _studyRoomRepository = studyRoomRepository;
            _userRepository = userRepository;
            _videoProvider = videoProvider;
        }

        public async Task ExecuteAsync(EnterStudyRoomCommand message, CancellationToken token)
        {
            var studyRoom = await _studyRoomRepository.GetAsync(message.StudyRoomId, token);
            if (studyRoom == null)
            {
                throw new ArgumentNullException(nameof(message.StudyRoomId), message.StudyRoomId.ToString());
            }

            var currentSession = studyRoom.GetCurrentSession();
            if (currentSession != null)
            {
                var user = await _userRepository.LoadAsync(message.UserId, token);
                studyRoom.AddUserToStudyRoom(user);

                var roomAvailable = await _videoProvider.GetRoomAvailableAsync(currentSession.SessionId);
                if (roomAvailable)
                {
                    var jwt = _videoProvider.CreateRoomToken(
                        currentSession.SessionId,
                       user.Id, user.Name);
                    message.JwtToken = jwt;
                }
            }
        }
    }
}