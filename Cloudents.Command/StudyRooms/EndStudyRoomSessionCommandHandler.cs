using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.StudyRooms
{
    public class EndStudyRoomSessionCommandHandler : ICommandHandler<EndStudyRoomSessionCommand>
    {
        private readonly IRepository<StudyRoom> _studyRoomRepository;
        public EndStudyRoomSessionCommandHandler(IRepository<StudyRoom> studyRoomRepository)
        {
            _studyRoomRepository = studyRoomRepository;
        }

        public async Task ExecuteAsync(EndStudyRoomSessionCommand message, CancellationToken token)
        {
            var room = await _studyRoomRepository.LoadAsync(message.StudyRoomId, token);
            if (room.Identifier.Split('_').All(a => a != message.UserId.ToString()))
            {
                throw new ArgumentException();
            }

            var session = room.GetCurrentSession();
            session?.EndSession();

        }
    }

    //public class StudyRoomVideoReadyCommandHandler :  ICommandHandler<StudyRoomVideoReadyCommand>
    //{
    //    private readonly IRepository<StudyRoom> _studyRoomSession;

    //    public StudyRoomVideoReadyCommandHandler(IRepository<StudyRoom> studyRoomSession)
    //    {
    //        _studyRoomSession = studyRoomSession;
    //    }

    //    public async Task ExecuteAsync(StudyRoomVideoReadyCommand message, CancellationToken token)
    //    {
    //        var room = await _studyRoomSession.LoadAsync(message.StudyRoomId,token);
    //        var session = room.Sessions.AsQueryable().Single(s => s.SessionId == message.SessionIdentifier);
    //        session.UpdateVideo();
    //    }
    //}


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