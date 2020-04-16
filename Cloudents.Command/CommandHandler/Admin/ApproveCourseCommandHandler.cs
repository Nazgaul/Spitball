using Cloudents.Command.Command.Admin;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.CommandHandler.Admin
{
    public class ApproveCourseCommandHandler : ICommandHandler<ApproveCourseCommand>
    {
        private readonly IRepository<Course2> _courseRepository;
        public ApproveCourseCommandHandler(IRepository<Course2> courseRepository
            )
        {
            _courseRepository = courseRepository;
        }

        public async Task ExecuteAsync(ApproveCourseCommand message, CancellationToken token)
        {
            var course = await _courseRepository.LoadAsync(message.Course, token);
            //var subject = await _subjectRepository.GetCourseSubjectByName(message.Subject
            //        , token);
            //course.SetSchoolType(message.SchoolType);
            course.Approve();
            //course.SetSubject(subject);
        }
    }
}
