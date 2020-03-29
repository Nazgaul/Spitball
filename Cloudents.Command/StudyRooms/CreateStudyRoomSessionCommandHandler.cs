using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.StudyRooms
{
    public class CreateStudyRoomSessionCommandHandler : ICommandHandler<CreateStudyRoomSessionCommand, CreateStudyRoomSessionCommandResult>
    {
        private readonly IRepository<StudyRoom> _studyRoomRepository;
        private readonly IVideoProvider _videoProvider;
        public CreateStudyRoomSessionCommandHandler(IRepository<StudyRoom> studyRoomRepository, IVideoProvider videoProvider)
        {
            _studyRoomRepository = studyRoomRepository;
            _videoProvider = videoProvider;
        }

        public async Task<CreateStudyRoomSessionCommandResult> ExecuteAsync(CreateStudyRoomSessionCommand message, CancellationToken token)
        {
            var room = await _studyRoomRepository.LoadAsync(message.StudyRoomId, token);
            if (room.Tutor.Id != message.UserId) //only tutor can open a session
            {
                throw new ArgumentException("Only tutor can open study room");
            }

            var lastSession = room.GetCurrentSession();
            if (lastSession != null)
            {
                var roomAvailable = await _videoProvider.GetRoomAvailableAsync(lastSession.SessionId);
                if (roomAvailable)
                {
                    lastSession.ReJoinStudyRoom();
                    var jwtToken2 = _videoProvider.CreateRoomToken(lastSession.SessionId, message.UserId);
                    return  new CreateStudyRoomSessionCommandResult(jwtToken2);
                }
                lastSession.EndSession();
            }
            var sessionName = $"{message.StudyRoomId}_{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}";

            await _videoProvider.CreateRoomAsync(sessionName,
                room.Tutor.User.Country,
                message.RecordVideo,
                message.CallbackUrl,
                room.Type.GetValueOrDefault(StudyRoomType.PeerToPeer)
                );
            var session = new StudyRoomSession(room, sessionName);
            room.AddSession(session);
            var jwtToken = _videoProvider.CreateRoomToken(sessionName, message.UserId);
            return new CreateStudyRoomSessionCommandResult(jwtToken);
        }
    }
}
