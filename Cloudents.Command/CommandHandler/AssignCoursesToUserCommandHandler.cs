using Cloudents.Command.Command;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.CommandHandler
{
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Ioc inject")]
    public class AssignCoursesToUserCommandHandler : ICommandHandler<AssignCoursesToUserCommand>
    {
        private readonly ICourseRepository _courseRepository;
        private readonly IRepository<RegularUser> _userRepository;

        public AssignCoursesToUserCommandHandler(ICourseRepository courseRepository,
            IRepository<RegularUser> userRepository)
        {
            _courseRepository = courseRepository;
            _userRepository = userRepository;
        }


        public async Task ExecuteAsync(AssignCoursesToUserCommand message, CancellationToken token)
        {
            var user = await _userRepository.LoadAsync(message.UserId, token);
            //user.Courses.Clear();

            var courses = message.Name.Select(s => _courseRepository.GetOrAddAsync(s, token));

            user.AssignCourses(await Task.WhenAll(courses));
            //foreach (var name in message.Name)
            //{
            //    var course = await _courseRepository.GetOrAddAsync(name, token);
            //    if (user.Courses.Any(a => a.Course == course))
            //    {
            //        continue;
                    
            //    }
            //    var p = new UserCourse(user,course);
            //    if (user.Courses.Add(p))
            //    {
            //        course.Count++;
            //        await _courseRepository.UpdateAsync(course, token);
            //    }
            //}

            await _userRepository.UpdateAsync(user, token);
        }
    }
}