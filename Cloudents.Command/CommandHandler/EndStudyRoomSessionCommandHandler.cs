using Cloudents.Command.Command;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.CommandHandler
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
            var room = await _studyRoomRepository.GetAsync(message.StudyRoomId, token);
            var session = room.Sessions.Where(w => w.Id == message.StudyRoomSessionId).FirstOrDefault();
            StudyRoomSession.EndSession(session);
        }
    }
}
