using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.Courses
{
    public class TeachCourseCommandHandler : ICommandHandler<TeachCourseCommand>
    {
        private readonly IRepository<RegularUser> _userRepository;
        public TeachCourseCommandHandler(IRepository<RegularUser> userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task ExecuteAsync(TeachCourseCommand message, CancellationToken token)
        {
            var user = await _userRepository.LoadAsync(message.UserId, token);
            var course = user.UserCourses.First(w => w.Course.Id == message.Name);
            course.CanTeach = !course.CanTeach;
            await _userRepository.UpdateAsync(user, token);
        }
    }
}
