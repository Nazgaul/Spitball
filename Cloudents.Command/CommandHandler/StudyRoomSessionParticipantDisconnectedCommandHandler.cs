using Cloudents.Command.Command;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.CommandHandler
{
    public class StudyRoomSessionParticipantDisconnectedCommandHandler : ICommandHandler<StudyRoomSessionParticipantDisconnectedCommand>
    {

        private readonly IRepository<SessionParticipantDisconnect> _sessionDisconnectRepository;
        private readonly IRepository<StudyRoom> _studyRoomRepository;
        public StudyRoomSessionParticipantDisconnectedCommandHandler(IRepository<SessionParticipantDisconnect> sessionDisconnectRepository,
            IRepository<StudyRoom> studyRoomRepository)
        {
            _sessionDisconnectRepository = sessionDisconnectRepository;
            _studyRoomRepository = studyRoomRepository;
        }


        public async Task ExecuteAsync(StudyRoomSessionParticipantDisconnectedCommand message, CancellationToken token)
        {
            var studyRoom = await _studyRoomRepository.LoadAsync(message.RoomId, token);
           
            var studyRoomSession = studyRoom.GetCurrentSession();
            if (studyRoomSession == null)
            {
                return;
            }
           
            var sessionParticipant = studyRoomSession.ParticipantDisconnections.FirstOrDefault();


            if (sessionParticipant == null)
            {
                var sessionDisconnect = new SessionParticipantDisconnect(studyRoomSession);
                await _sessionDisconnectRepository.AddAsync(sessionDisconnect, token);
            }
           
        }
    }
}
