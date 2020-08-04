using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Command.Command;
using Cloudents.Core.Entities;
using Cloudents.Core.Exceptions;
using Cloudents.Core.Interfaces;

namespace Cloudents.Command.CommandHandler
{
    public class DeleteStudyRoomCommandHandler : ICommandHandler<DeleteStudyRoomCommand>
    {
        private readonly IRepository<StudyRoom> _studyRoomRepository;
        private readonly IRepository<Course> _courseRepository;

        public DeleteStudyRoomCommandHandler(IRepository<StudyRoom> studyRoomRepository, IRepository<Course> courseRepository)
        {
            _studyRoomRepository = studyRoomRepository;
            _courseRepository = courseRepository;
        }

        public async Task ExecuteAsync(DeleteStudyRoomCommand message, CancellationToken token)
        {
            var studyRoom = await _studyRoomRepository.GetAsync(message.Id, token);
            if (studyRoom is null)
            {
                throw new NotFoundException();
            }
            if (studyRoom.Tutor.Id != message.UserId)
            {
                throw new UnauthorizedAccessException();
            }
            
            if (studyRoom.Sessions.Any())
            {
                throw new ArgumentException();
            }

            if (studyRoom is BroadCastStudyRoom p)
            {
                if (p.Course.Documents.Any())
                {
                    throw new ArgumentException();

                }

                await _courseRepository.DeleteAsync(p.Course, token);

            }
            else
            {
                await _studyRoomRepository.DeleteAsync(studyRoom, token);
            }
        }
    }
}