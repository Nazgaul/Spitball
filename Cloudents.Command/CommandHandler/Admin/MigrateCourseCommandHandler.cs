using Cloudents.Command.Command.Admin;
using Cloudents.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.CommandHandler.Admin
{
    public class MigrateCourseCommandHandler : ICommandHandler<MigrateCourseCommand>
    {
        private readonly ICourseRepository _courseRepository;
        public MigrateCourseCommandHandler(ICourseRepository courseRepository)
        {
            _courseRepository = courseRepository;
        }

        public async Task ExecuteAsync(MigrateCourseCommand message, CancellationToken token)
        {
            await _courseRepository.MigrateCourseAsync(message.CourseToKeep, message.CourseToRemove, token);
        }
    }
}
