using Cloudents.Command.Command.Admin;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.CommandHandler.Admin
{
    public class DeleteSubjectCommandHandler : ICommandHandler<DeleteSubjectCommand>
    {
        private readonly IRepository<CourseSubject> _courseRepository;
        public DeleteSubjectCommandHandler(IRepository<CourseSubject> courseRepository)
        {
            _courseRepository = courseRepository;
        }

        public async Task ExecuteAsync(DeleteSubjectCommand message, CancellationToken token)
        {
            var course = await _courseRepository.GetAsync(message.SubjectId, token);
            await _courseRepository.DeleteAsync(course, token);
        }
    }
}
