using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.Courses
{
    public class TeachCourseCommandHandler : ICommandHandler<TeachCourseCommand>
    {
        private readonly IRepository<User> _userRepository;
        public TeachCourseCommandHandler(IRepository<User> userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task ExecuteAsync(TeachCourseCommand message, CancellationToken token)
        {
            var user = await _userRepository.LoadAsync(message.UserId, token);
            //var course = user.UserCourses.AsQueryable().Single(w => w.Course.Id == message.Name);
            user.CanTeachCourse(message.Name);
            await _userRepository.UpdateAsync(user, token);
        }
    }
}
