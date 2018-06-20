using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Command;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using JetBrains.Annotations;

namespace Cloudents.Core.CommandHandler
{
    [UsedImplicitly]
    public class CreateCourseCommandHandler : ICommandHandler<CreateCourseCommand, CreateCourseCommandResult>
    {
        private readonly ICourseRepository _courseRepository;
        private readonly IRepository<University> _universityRepository;

        public CreateCourseCommandHandler(ICourseRepository courseRepository, IRepository<University> universityRepository)
        {
            _courseRepository = courseRepository;
            _universityRepository = universityRepository;
        }

        public async Task<CreateCourseCommandResult> ExecuteAsync(CreateCourseCommand command, CancellationToken token)
        {
            var university = await _universityRepository.LoadAsync(command.UniversityId, token).ConfigureAwait(true);

            var course = await _courseRepository.GetCourseAsync(command.UniversityId, command.Name, token);

            if (course != null) return new CreateCourseCommandResult(course.Id);
            course = new Course(command.Name, university);

            var id = await _courseRepository.AddAsync(course, token).ConfigureAwait(true);
            return new CreateCourseCommandResult((long)id);
        }
    }
}
