using Cloudents.Command.Command;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.CommandHandler
{
    public class CreateStudyRoomSessionCommandHandler : ICommandHandler<CreateStudyRoomSessionCommand>
    {
        private readonly IRepository<StudyRoom> _studyRoomRepository;
        public CreateStudyRoomSessionCommandHandler(IRepository<StudyRoom> studyRoomRepository)
        {
            _studyRoomRepository = studyRoomRepository;
        }

        public async Task ExecuteAsync(CreateStudyRoomSessionCommand message, CancellationToken token)
        {
            var room = await _studyRoomRepository.LoadAsync(message.StudyRoomId, token);
            if (room.Tutor.Id != message.TutorId)
            {
                throw new ArgumentException();
            }

            if (room.Sessions.Any(a => a.Ended == null))
            {
                throw new ArgumentException("there is already open session");
            }
            var session = new StudyRoomSession(room, message.SessionName);
            room.AddSession(session);
        }
    }
}
