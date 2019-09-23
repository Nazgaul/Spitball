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

            var session = room.Sessions.AsQueryable().Single(w => w.Ended == null);
            //foreach (var session in sessions)
            //{
            if (session.EndSession())
            {
                  await _videoProvider.CloseRoomAsync(session.SessionId);
            }
            //}


        }
    }


    public class EndStudyRoomSessionTwilioCommandHandler : ICommandHandler<EndStudyRoomSessionTwilioCommand>
    {
        private readonly IRepository<StudyRoom> _studyRoomRepository;
        public EndStudyRoomSessionTwilioCommandHandler(IRepository<StudyRoom> studyRoomRepository)
        {
            _studyRoomRepository = studyRoomRepository;
        }


        public async Task ExecuteAsync(EndStudyRoomSessionTwilioCommand message, CancellationToken token)
        {
            var room = await _studyRoomRepository.LoadAsync(message.StudyRoomId, token);
            var sessionEnded = room.Sessions.AsQueryable().FirstOrDefault(w => w.SessionId == message.StudyRoomSessionId);

            sessionEnded?.EndSession();

        }
    }
}