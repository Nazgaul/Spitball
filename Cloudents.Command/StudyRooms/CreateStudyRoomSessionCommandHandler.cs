﻿using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;

namespace Cloudents.Command.StudyRooms
{
    public class CreateStudyRoomSessionCommandHandler : ICommandHandler<CreateStudyRoomSessionCommand>
    {
        private readonly IRepository<StudyRoom> _studyRoomRepository;
        private readonly IVideoProvider _videoProvider;
        public CreateStudyRoomSessionCommandHandler(IRepository<StudyRoom> studyRoomRepository, IVideoProvider videoProvider)
        {
            _studyRoomRepository = studyRoomRepository;
            _videoProvider = videoProvider;
        }

        public async Task ExecuteAsync(CreateStudyRoomSessionCommand message, CancellationToken token)
        {
            var room = await _studyRoomRepository.LoadAsync(message.StudyRoomId, token);
            if (room.Tutor.Id != message.UserId) //only tutor can open a session
            {
                throw new ArgumentException();
            }

            var sessionName = $"{message.StudyRoomId}_{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}";
            var lastSession = room.Sessions.LastOrDefault();
            if (lastSession != null && lastSession.Ended == null)
            {
                var roomAvailable = await _videoProvider.GetRoomAvailableAsync(lastSession.SessionId);
                if (roomAvailable)
                {
                    lastSession.ReJoinStudyRoom();
                    return;
                }

            }
            await _videoProvider.CreateRoomAsync(sessionName, message.RecordVideo);
            var session = new StudyRoomSession(room, sessionName);
            room.AddSession(session);
        }
    }
}
