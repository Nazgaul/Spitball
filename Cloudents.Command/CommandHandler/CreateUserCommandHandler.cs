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
        private readonly IRepository<University> _universityRepository;
        private readonly IRepository<Course> _courseRepository;

        public CreateUserCommandHandler(IRegularUserRepository userRepository, IRepository<University> universityRepository, IRepository<Course> courseRepository)
        {
            _userRepository = userRepository;
            _universityRepository = universityRepository;
            _courseRepository = courseRepository;
        }

        public async Task ExecuteAsync(CreateUserCommand message, CancellationToken token)
        {
            if (message.UniversityId.HasValue)
            {
                var university = await _universityRepository.LoadAsync(message.UniversityId.Value, token);
                message.User.SetUniversity(university);
            }

            if (!string.IsNullOrEmpty(message.Course))
            {
                var course = await _courseRepository.LoadAsync(message.Course, token);
                message.User.AssignCourses(new[] { course });
            }
            await _userRepository.AddAsync(message.User, token);
        }
    }
}