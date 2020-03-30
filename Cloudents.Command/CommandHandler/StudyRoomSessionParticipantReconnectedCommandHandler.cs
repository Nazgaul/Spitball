using System;
using Cloudents.Command.Command;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.CommandHandler
{
    public class StudyRoomSessionUserConnectedCommandHandler : ICommandHandler<StudyRoomSessionUserConnectedCommand>
    {
        //private readonly IRepository<SessionParticipantDisconnect> _sessionDisconnectRepository;
        private readonly IRepository<StudyRoomSession> _studyRoomRepository;
        private readonly IRegularUserRepository _userRepository;

        public StudyRoomSessionUserConnectedCommandHandler(IRepository<StudyRoomSession> studyRoomRepository, IRegularUserRepository userRepository)
        {
            _studyRoomRepository = studyRoomRepository;
            _userRepository = userRepository;
        }


        public async Task ExecuteAsync(StudyRoomSessionUserConnectedCommand message, CancellationToken token)
        {
            var studyRoomSession = await _studyRoomRepository.LoadAsync(message.SessionId, token);

            if (studyRoomSession.StudyRoom.Id != message.RoomId)
            {
                throw new ArgumentException();
            }

            var user = await _userRepository.LoadAsync(message.UserId, token);
            studyRoomSession.AddUser(user);
          
        }
    }
}
