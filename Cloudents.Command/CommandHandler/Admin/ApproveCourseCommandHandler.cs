using Cloudents.Command.Command.Admin;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.CommandHandler.Admin
{
    public class ApproveCourseCommandHandler : ICommandHandler<ApproveCourseCommand>
    {
        private readonly IRepository<Course> _courseRepository;
        private readonly ICourseSubjectRepository _subjectRepository;
        public ApproveCourseCommandHandler(IRepository<Course> courseRepository,
            ICourseSubjectRepository subjectRepository)
        {
            _courseRepository = courseRepository;
            _subjectRepository = subjectRepository;
        }

        public async Task ExecuteAsync(ApproveCourseCommand message, CancellationToken token)
        {
            var course = await _courseRepository.LoadAsync(message.Course, token);
            var subject = await _subjectRepository.GetCourseSubjectByName(message.Subject
                    , token);
            course.SetShcoolType(message.SchoolType);
            course.Approve();
            course.SetSubject(subject);
        }
    }
}
