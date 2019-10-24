using Cloudents.Command.Command.Admin;
using Cloudents.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
