using Cloudents.Command.Command;
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

        public CreateUserCommandHandler(IRegularUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public Task ExecuteAsync(CreateUserCommand message, CancellationToken token)
        {
            //if (!string.IsNullOrEmpty(message.Course))
            //{
            //    var course = await _courseRepository.GetAsync(message.Course, token);
            //    if (course != null)
            //    {
            //        message.User.AssignCourses(new[] {course});
            //    }
            //}
            return _userRepository.AddAsync(message.User, token);
        }
    }
}