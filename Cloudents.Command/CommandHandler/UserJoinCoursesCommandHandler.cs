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
    public class UserJoinCoursesCommandHandler : ICommandHandler<UserJoinCoursesCommand>
    {
        private readonly ICourseRepository _courseRepository;
        private readonly IRepository<RegularUser> _userRepository;

        public UserJoinCoursesCommandHandler(ICourseRepository courseRepository,
            IRepository<RegularUser> userRepository)
        {
            _courseRepository = courseRepository;
            _userRepository = userRepository;
        }


        public async Task ExecuteAsync(UserJoinCoursesCommand message, CancellationToken token)
        {
            var user = await _userRepository.LoadAsync(message.UserId, token);
            var courses = message.Name.Select(s => _courseRepository.LoadAsync(s, token));
            user.AssignCourses(await Task.WhenAll(courses));
            await _userRepository.UpdateAsync(user, token);
        }
    }
}