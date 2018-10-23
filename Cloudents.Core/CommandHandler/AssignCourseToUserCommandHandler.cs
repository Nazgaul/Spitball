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
        private readonly IRepository<Course> _courseRepository;
        private readonly IRepository<User> _userRepository;

        public AssignCourseToUserCommandHandler(IRepository<Course> courseRepository, IRepository<User> userRepository)
        {
            _courseRepository = courseRepository;
            _userRepository = userRepository;
        }


        public async Task ExecuteAsync(AssignCourseToUserCommand message, CancellationToken token)
        {
            var user = await _userRepository.LoadAsync(message.UserId, token);
            var course = await _courseRepository.GetAsync(message.Name, token);

            if (course == null)
            {
                course = new Course(message.Name);
                await _courseRepository.AddAsync(course, token).ConfigureAwait(true);
            }

            if (user.Courses.Add(course))
            {
                course.Count++;
            }

            await _courseRepository.UpdateAsync(course, token);
            await _userRepository.UpdateAsync(user, token);
        }
    }
}