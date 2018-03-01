using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Command;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using JetBrains.Annotations;

namespace Cloudents.Core.CommandHandler
{
    [UsedImplicitly]
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
            var university = await _universityRepository.LoadAsync(command.UniversityId, token).ConfigureAwait(true);

            var course = _courseRepository.GetQueryable()
                .Where(w => w.Name == command.Name && w.University.Id == command.UniversityId)
                .Take(1).ToList().FirstOrDefault();
            if (course != null) return new CreateCourseCommandResult(course.Id);
            course = new Course(command.Name, university);

            var id = await _courseRepository.SaveAsync(course, token).ConfigureAwait(true);
            return new CreateCourseCommandResult((long)id);
        }
    }
}
