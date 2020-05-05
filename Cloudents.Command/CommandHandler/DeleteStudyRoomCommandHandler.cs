using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Command.Command;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;

namespace Cloudents.Command.CommandHandler
{
    public class DeleteStudyRoomCommandHandler : ICommandHandler<DeleteStudyRoomCommand>
    {
        private readonly IRepository<StudyRoom> _studyRoomRepository;

        public DeleteStudyRoomCommandHandler(IRepository<StudyRoom> studyRoomRepository)
        {
            _studyRoomRepository = studyRoomRepository;
        }

        public async Task ExecuteAsync(DeleteStudyRoomCommand message, CancellationToken token)
        {
            var studyRoom = await _studyRoomRepository.LoadAsync(message.Id, token);
            if (studyRoom.Tutor.Id != message.UserId)
            {
                throw new UnauthorizedAccessException();
            }
            
            if (studyRoom.Sessions.Any())
            {
                throw new ArgumentException();
            }

            await _studyRoomRepository.DeleteAsync(studyRoom,token);
        }
    }
}