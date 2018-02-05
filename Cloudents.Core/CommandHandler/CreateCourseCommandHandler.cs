using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Command;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.CommandHandler
{
    public class CreateCourseCommandHandler : ICommandHandlerAsync<CreateCourseCommand, CreateCourseCommandResult>
    {
        private readonly IRepository<Course> _courseRepository;
        private readonly IRepository<University> _universityRepository;

        public CreateCourseCommandHandler(IRepository<Course> courseRepository, IRepository<University> universityRepository)
        {
            _courseRepository = courseRepository;
            _universityRepository = universityRepository;
        }

        public async Task<CreateCourseCommandResult> ExecuteAsync(CreateCourseCommand command, CancellationToken token)
        {
            var university = await _universityRepository.LoadAsync(command.UniversityId, token).ConfigureAwait(false);
            var course = new Course(command.Name, university);

            var id = await _courseRepository.AddAsync(course, token).ConfigureAwait(false);

            return new CreateCourseCommandResult((long)id);
        }
    }
}
