using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using System;
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
            if (room.Tutor.Id != message.UserId)
            {
                throw new ArgumentException();
            }
            var session = room.GetCurrentSession();
            session?.EndSession();
        }
    }
}