using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Storage;

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

    public class UploadStudyRoomVideoCommandHandler : ICommandHandler<UploadStudyRoomVideoCommand>
    {
        private readonly IRepository<StudyRoom> _studyRoomRepository;
        private readonly IStudyRoomSessionBlobProvider _blobProvider;

        public UploadStudyRoomVideoCommandHandler(IRepository<StudyRoom> studyRoomRepository, IStudyRoomSessionBlobProvider blobProvider)
        {
            _studyRoomRepository = studyRoomRepository;
            _blobProvider = blobProvider;
        }

        public async Task ExecuteAsync(UploadStudyRoomVideoCommand message, CancellationToken token)
        {
            var room = await _studyRoomRepository.LoadAsync(message.StudyRoomId, token);
            if (room.Identifier.Split('_').All(a => a != message.UserId.ToString()))
            {
                throw new ArgumentException();
            }

            var session = room.Sessions.AsQueryable().OrderByDescending(o => o.Ended).Take(1).SingleOrDefault();
            if (session is null)
            {
                throw new ArgumentException();
            }

            session.UpdateVideo();
            await _blobProvider.UploadVideoAsync(room.Id, session.SessionId, message.VideoStream, token);
            //await _blobProvider.UploadStreamAsync($"{room.Id}/{session.SessionId}", 
            //                                message.VideoStream, token: token);
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