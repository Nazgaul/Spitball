using Cloudents.Command.Command;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.CommandHandler
{
    public class StudyRoomSessionParticipantReconnectedCommandHandler : ICommandHandler<StudyRoomSessionParticipantReconnectedCommand>
    {
        private readonly IRepository<SessionParticipantDisconnect> _sessionDisconnectRepository;
        private readonly IRepository<StudyRoom> _studyRoomRepository;
        public StudyRoomSessionParticipantReconnectedCommandHandler(IRepository<SessionParticipantDisconnect> sessionDisconnectRepository,
            IRepository<StudyRoom> studyRoomRepository)
        {
            _sessionDisconnectRepository = sessionDisconnectRepository;
            _studyRoomRepository = studyRoomRepository;
        }

        public async Task ExecuteAsync(StudyRoomSessionParticipantReconnectedCommand message, CancellationToken token)
        {
            var studyRoom = await _studyRoomRepository.GetAsync(message.Id, token);
            var studyRoomSession = studyRoom.GetCurrentSession();
            if (studyRoomSession is null)
            {
                return;
            }
            var sessionParticipant = studyRoomSession.ParticipantDisconnections.FirstOrDefault();

            if (sessionParticipant != null)
            {
                await _sessionDisconnectRepository.DeleteAsync(sessionParticipant, token);
            }
        }
    }
}
