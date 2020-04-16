using Cloudents.Command.Command;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.CommandHandler
{
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Ioc resolve")]
    public class CreateUserCommandHandler : ICommandHandler<CreateUserCommand>
    {
        private readonly IRegularUserRepository _userRepository;
        private readonly ICourseRepository _courseRepository;

        public CreateUserCommandHandler(IRegularUserRepository userRepository, ICourseRepository courseRepository)
        {
            _userRepository = userRepository;
            _courseRepository = courseRepository;
        }

        public async Task ExecuteAsync(CreateUserCommand message, CancellationToken token)
        {
            if (!string.IsNullOrEmpty(message.Course))
            {
                var course = await _courseRepository.GetCourseByName(message.Course, token);
                if (course != null)
                {
                    message.User.AssignCourses(new[] {course});
                }
            }
            await _userRepository.AddAsync(message.User, token);
        }
    }
}