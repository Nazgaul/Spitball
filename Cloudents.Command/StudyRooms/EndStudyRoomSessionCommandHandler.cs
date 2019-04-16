using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;

namespace Cloudents.Command.StudyRooms
{
    public class EndStudyRoomSessionCommandHandler : ICommandHandler<EndStudyRoomSessionCommand>
    {
        private readonly IRepository<StudyRoom> _studyRoomRepository;
        private readonly IVideoProvider _videoProvider;
        public EndStudyRoomSessionCommandHandler(IRepository<StudyRoom> studyRoomRepository, IVideoProvider videoProvider)
        {
            _studyRoomRepository = studyRoomRepository;
            _videoProvider = videoProvider;
        }

        public async Task ExecuteAsync(EndStudyRoomSessionCommand message, CancellationToken token)
        {
            var room = await _studyRoomRepository.LoadAsync(message.StudyRoomId, token);
            if (room.Identifier.Split('_').All(a => a != message.UserId.ToString()))
            {
                throw new ArgumentException();
            }

            var session = room.Sessions.Last();
            session.EndSession();
            await _videoProvider.CloseRoomAsync(session.SessionId);
           
        }
    }
}