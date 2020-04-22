using System.Threading;
using System.Threading.Tasks;
using Cloudents.Command.Command.Admin;
using Cloudents.Core.Interfaces;

namespace Cloudents.Command.CommandHandler.Admin
{
    public class RenameCourseCommandHandler : ICommandHandler<RenameCourseCommand>
    {
        private readonly ICourseRepository _courseRepository;
        public RenameCourseCommandHandler(ICourseRepository courseRepository)
        {
            _courseRepository = courseRepository;
        }

        public async Task ExecuteAsync(RenameCourseCommand message, CancellationToken token)
        {
            await _courseRepository.RenameCourseAsync(message.CourseName, message.NewName, token);
        }
    }
}