using Cloudents.Command.Command.Admin;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.CommandHandler.Admin
{
    public class DeleteCourseCommandHandler : ICommandHandler<DeleteCourseCommand>
    {
        private readonly IRepository<Course> _courseRepository;

        public DeleteCourseCommandHandler(IRepository<Course> courseRepository)
        {
            _courseRepository = courseRepository;
        }

        public async Task ExecuteAsync(DeleteCourseCommand message, CancellationToken token)
        {
            var courseToRemove = await _courseRepository.LoadAsync(message.CourseToRemove, token);
            await _courseRepository.DeleteAsync(courseToRemove, token);
        }
    }
}
