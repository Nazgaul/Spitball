using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Command;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.CommandHandler
{
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Ioc inject")]
    public class AssignCourseToUserCommandHandler : ICommandHandler<AssignCourseToUserCommand>
    {
        private readonly ICourseRepository _courseRepository;
        private readonly IRepository<User> _userRepository;

        public AssignCourseToUserCommandHandler(ICourseRepository courseRepository, IRepository<User> userRepository)
        {
            _courseRepository = courseRepository;
            _userRepository = userRepository;
        }


        public async Task ExecuteAsync(AssignCourseToUserCommand message, CancellationToken token)
        {
            var user = await _userRepository.LoadAsync(message.UserId, token);
            var course = await _courseRepository.GetOrAddAsync(message.Name, token);
            if (user.Courses.Add(course))
            {
                course.Count++;
            }

            await _courseRepository.UpdateAsync(course, token);
            await _userRepository.UpdateAsync(user, token);
        }
    }
}