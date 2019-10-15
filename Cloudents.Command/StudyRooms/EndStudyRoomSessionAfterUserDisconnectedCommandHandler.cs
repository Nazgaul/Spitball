using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.StudyRooms
{
    public class EndStudyRoomSessionAfterUserDisconnectedCommandHandler : ICommandHandler<EndStudyRoomSessionAfterUserDisconnectedCommand>
    {
        private readonly IRepository<StudyRoom> _studyRoomRepository;
        private readonly IRepository<SessionParticipantDisconnect> _sessionParticipantDisconnectRepository;
        public EndStudyRoomSessionAfterUserDisconnectedCommandHandler(IRepository<SessionParticipantDisconnect> sessionParticipantDisconnectRepository, IRepository<StudyRoom> studyRoomRepository)
        {
            _studyRoomRepository = studyRoomRepository;
            _sessionParticipantDisconnectRepository = sessionParticipantDisconnectRepository;
        }

        public async Task ExecuteAsync(EndStudyRoomSessionAfterUserDisconnectedCommand message, CancellationToken token)
        {
            var sessionParticipant = await _sessionParticipantDisconnectRepository.GetAsync(message.SessionParticipantDisconnectId, token);
            if (sessionParticipant != null)
            {
                var room = await _studyRoomRepository.LoadAsync(sessionParticipant.StudyRoomSession.StudyRoom.Id, token);

                var session = room.GetCurrentSession();
                session.EndSession();
            }
        }
    }
}
