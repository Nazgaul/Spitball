using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Command;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.CommandHandler
{
    public class CreateCourseCommandHandler : ICommandHandler<CreateCourseCommand>
    {
        private readonly ICourseRepository _courseRepository;
        private readonly IRepository<University> _universityRepository;

        public CreateCourseCommandHandler(ICourseRepository courseRepository, IRepository<University> universityRepository)
        {
            _courseRepository = courseRepository;
            _universityRepository = universityRepository;
        }

        public async Task ExecuteAsync(CreateCourseCommand message, CancellationToken token)
        {
            var university = await _universityRepository.LoadAsync(message.UniversityId, token).ConfigureAwait(true);

            var course = await _courseRepository.GetCourseAsync(message.UniversityId, message.Name, token);

            if (course != null)
            {
                message.Id = course.Id;
                return;
            }
            course = new Course(message.Name, university);

            message.Id = (long)await _courseRepository.AddAsync(course, token).ConfigureAwait(true);
            //return new CreateCourseCommandResult(message.Id);
        }
    }
}