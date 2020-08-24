using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Enum;
using Cloudents.Core.Extension;
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
            var list = tutor.Courses;
            var newPosition = message.NewPosition;
            var oldPosition = message.OldPosition;
            if (message.ModelVisibleOnly)

            {
                newPosition=  list.Select((s, i) => new {s, i})
                    .Where(w => w.s.State == ItemState.Ok).Skip(message.NewPosition).First().i;

                oldPosition=  list.Select((s, i) => new {s, i})
                    .Where(w => w.s.State == ItemState.Ok).Skip(message.OldPosition).First().i;
            }
            list.Move(oldPosition,newPosition);
            //var newIndex = message.NewPosition;

            //var item = tutor.Courses[oldIndex];
            //list.RemoveAt(oldIndex);
            //if (newIndex > oldIndex) newIndex--; 
            //list.Insert(newIndex, item);
        }
    }


    //public class UpdateDocumentPositionCommandHandler : ICommandHandler<UpdateDocumentPositionCommand>
    //{
    //    private readonly ICourseRepository _courseRepository;


    //    public UpdateDocumentPositionCommandHandler(ICourseRepository courseRepository)
    //    {
    //        _courseRepository = courseRepository;
    //    }

    //    public async Task ExecuteAsync(UpdateDocumentPositionCommand message, CancellationToken token)
    //    {
    //        var course = await _courseRepository.GetAsync(message.CourseId, token);
    //        if (course is null)
    //        {
    //            throw new ArgumentException();
    //        }

    //        if (course.Tutor.Id != message.TutorId)
    //        {
    //            throw new UnauthorizedAccessException();
    //        }
    //        var oldIndex = message.OldPosition;
    //        var list = course.Documents;
    //        var newIndex = message.NewPosition;

    //        var item = tutor.Courses[oldIndex];
    //        list.RemoveAt(oldIndex);
    //        if (newIndex > oldIndex) newIndex--; 
    //        list.Insert(newIndex, item);
    //    }
    //}
}