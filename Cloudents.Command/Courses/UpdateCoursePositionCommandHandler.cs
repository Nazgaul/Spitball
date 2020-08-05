using System;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Interfaces;

namespace Cloudents.Command.Courses
{
    public class UpdateCoursePositionCommandHandler : ICommandHandler<UpdateCoursePositionCommand>
    {
        private readonly ITutorRepository _tutorRepository;

        public UpdateCoursePositionCommandHandler(ITutorRepository tutorRepository)
        {
            _tutorRepository = tutorRepository;
        }

        public async Task ExecuteAsync(UpdateCoursePositionCommand message, CancellationToken token)
        {
            var tutor = await _tutorRepository.GetAsync(message.TutorId, token);
            if (tutor is null)
            {
                throw new ArgumentException();
            }
            var oldIndex = message.OldPosition;
            var list = tutor.Courses;
            var newIndex = message.NewPosition;

            var item = tutor.Courses[oldIndex];
            list.RemoveAt(oldIndex);
            if (newIndex > oldIndex) newIndex--; 
            list.Insert(newIndex, item);
        }
    }
}