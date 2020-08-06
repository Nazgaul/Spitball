using System;
using System.Threading;
using System.Threading.Tasks;
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
            list.Move(message.OldPosition, message.NewPosition);
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