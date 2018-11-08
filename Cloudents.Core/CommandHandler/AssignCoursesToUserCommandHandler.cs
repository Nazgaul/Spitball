using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Command;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Event;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.CommandHandler
{
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Ioc inject")]
    public class AssignCoursesToUserCommandHandler : ICommandHandler<AssignCoursesToUserCommand>
    {
        private readonly ICourseRepository _courseRepository;
        private readonly IRepository<User> _userRepository;

        public AssignCoursesToUserCommandHandler(ICourseRepository courseRepository, IRepository<User> userRepository)
        {
            _courseRepository = courseRepository;
            _userRepository = userRepository;
        }


        public async Task ExecuteAsync(AssignCoursesToUserCommand message, CancellationToken token)
        {
            var user = await _userRepository.LoadAsync(message.UserId, token);
            user.Courses.Clear();
            foreach (var name in message.Name)
            {
                var course = await _courseRepository.GetOrAddAsync(name, token);
                if (user.Courses.Add(course))
                {
                    course.Count++;
                    await _courseRepository.UpdateAsync(course, token);
                }
            }

            //this command handler only save courses and user courses. temp solution
            user.Events.Add(new UserChangeCoursesEvent(user));
            user.ForceEvent();
            await _userRepository.UpdateAsync(user, token);
        }
    }
}