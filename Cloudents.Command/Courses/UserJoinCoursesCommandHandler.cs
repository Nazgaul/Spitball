using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.Courses
{
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Ioc inject")]
    public class UserJoinCoursesCommandHandler : ICommandHandler<UserJoinCoursesCommand>
    {
        private readonly ICourseRepository _courseRepository;
        private readonly IRepository<User> _userRepository;

        public UserJoinCoursesCommandHandler(ICourseRepository courseRepository,
            IRepository<User> userRepository)
        {
            _courseRepository = courseRepository;
            _userRepository = userRepository;
        }


        public async Task ExecuteAsync(UserJoinCoursesCommand message, CancellationToken token)
        {
            var user = await _userRepository.LoadAsync(message.UserId, token);
            var courses = new List<Course2>();
            foreach (var s in message.Name)
            {
                var course = await _courseRepository.GetCourseByName(s, token);
                courses.Add(course);
            }

            user.AssignCourses(courses);
            await _userRepository.UpdateAsync(user, token);
        }
    }
}