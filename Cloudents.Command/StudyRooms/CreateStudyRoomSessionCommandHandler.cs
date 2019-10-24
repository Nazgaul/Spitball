using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

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
            var lastSession = room.GetCurrentSession();
            if (lastSession != null)
            {
                var roomAvailable = await _videoProvider.GetRoomAvailableAsync(lastSession.SessionId);
                if (roomAvailable)
                {
                    lastSession.ReJoinStudyRoom();
                    return;
                }
            }
            await _videoProvider.CreateRoomAsync(sessionName, message.RecordVideo, message.CallbackUrl, room.Type.GetValueOrDefault(StudyRoomType.PeerToPeer));
            var session = new StudyRoomSession(room, sessionName);
            room.AddSession(session);
        }
    }
}
