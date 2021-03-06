﻿using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.StudyRooms
{
    public class CreateStudyRoomSessionCommandHandler : ICommandHandler<CreateStudyRoomSessionCommand>
    {
        private readonly IRepository<StudyRoom> _studyRoomRepository;
        private readonly IStudyRoomProvider _videoProvider;
        private readonly IUrlBuilder _urlBuilder;
        public CreateStudyRoomSessionCommandHandler(IRepository<StudyRoom> studyRoomRepository, IStudyRoomProvider videoProvider, IUrlBuilder urlBuilder)
        {
            _studyRoomRepository = studyRoomRepository;
            _videoProvider = videoProvider;
            _urlBuilder = urlBuilder;
        }

        public async Task ExecuteAsync(CreateStudyRoomSessionCommand message, CancellationToken token)
        {
            var room = await _studyRoomRepository.LoadAsync(message.StudyRoomId, token);
            if (room.Tutor.Id != message.UserId) //only tutor can open a session
            {
                throw new ArgumentException("Only tutor can open study room");
            }

            var tutor = room.Tutor;
            var lastSession = room.GetCurrentSession();
            if (lastSession != null)
            {
                var roomAvailable = await _videoProvider.GetRoomAvailableAsync(lastSession.SessionId);
                if (roomAvailable)
                {
                    var jwtToken2 = _videoProvider.CreateRoomToken(lastSession.SessionId, tutor.Id, tutor.User.Name);
                    message.JwtToken = jwtToken2;
                    return;
                }
                lastSession.EndSession();
            }
            var sessionName = $"{message.StudyRoomId}_{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}";
            var url = _urlBuilder.BuildTwilioWebHookEndPoint(room.Id);
            await _videoProvider.CreateRoomAsync(sessionName,
                true,
                url,
                room.TopologyType
                );
            room.AddSession(sessionName);
            var jwtToken = _videoProvider.CreateRoomToken(sessionName, tutor.Id, tutor.User.Name);
            message.JwtToken = jwtToken;
        }
    }
}
