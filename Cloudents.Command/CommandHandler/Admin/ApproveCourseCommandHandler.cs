using Cloudents.Command.Command.Admin;
using Cloudents.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.CommandHandler.Admin
{
    public class ApproveCourseCommandHandler : ICommandHandler<ApproveCourseCommand>
    {
        private readonly ICourseRepository _courseRepository;
        public ApproveCourseCommandHandler(ICourseRepository courseRepository)
        {
            _courseRepository = courseRepository;
        }

        public async Task ExecuteAsync(ApproveCourseCommand message, CancellationToken token)
        {
            var course = await _courseRepository.LoadAsync(message.Course, token);
            course.Approve();
        }
    }
}
