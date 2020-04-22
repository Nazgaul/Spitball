using System.Threading;
using System.Threading.Tasks;
using Cloudents.Command.Command.Admin;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;

namespace Cloudents.Command.CommandHandler.Admin
{
    public class CreateCourseCommandHandler : ICommandHandler<CreateCourseCommand>
    {
        private readonly IRepository<Course> _courseRepository;
        private readonly ICourseSubjectRepository _courseSubjectRepository;

        public CreateCourseCommandHandler(IRepository<Course> courseRepository,
            ICourseSubjectRepository courseSubjectRepository)
        {
            _courseRepository = courseRepository;
            _courseSubjectRepository = courseSubjectRepository;
        }

        public async Task ExecuteAsync(CreateCourseCommand message, CancellationToken token)
        {
            var subject = await _courseSubjectRepository.LoadAsync(message.SubjectId, token);
            var course = new Course(message.CourseName,subject);
            await _courseRepository.AddAsync(course, token);
        }
    }
}