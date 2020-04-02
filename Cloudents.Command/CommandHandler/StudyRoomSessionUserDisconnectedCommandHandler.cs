using System;
using System.Linq;
using Cloudents.Command.Command;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.CommandHandler
{
    public class StudyRoomSessionUserDisconnectedCommandHandler : ICommandHandler<StudyRoomSessionUserDisconnectedCommand>
    {

        private readonly IRepository<StudyRoom> _studyRoomRepository;
        private readonly IRegularUserRepository _userRepository;

        public StudyRoomSessionUserDisconnectedCommandHandler(IRepository<StudyRoom> studyRoomRepository, IRegularUserRepository userRepository)
        {
            _studyRoomRepository = studyRoomRepository;
            _userRepository = userRepository;
        }


        public async Task ExecuteAsync(StudyRoomSessionUserDisconnectedCommand message, CancellationToken token)
        {
            var studyRoom = await _studyRoomRepository.LoadAsync(message.RoomId, token);
            var studyRoomSession = studyRoom.Sessions.AsQueryable().Single(s => s.SessionId == message.SessionId);
            var user = await _userRepository.LoadAsync(message.UserId, token);
            studyRoomSession.UserDisconnect(user, message.TimeInRoom);
        }

    }
}
