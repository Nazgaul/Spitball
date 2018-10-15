using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Command;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.CommandHandler
{
    [SuppressMessage("ReSharper", "UnusedMember.Global",Justification = "Ioc inject")]
    public class CreateCourseCommandHandler : ICommandHandler<CreateCourseCommand>
    {
        private readonly ICourseRepository _courseRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<UserCourseRelationship> _userCourseRelationshipRepository;

        public CreateCourseCommandHandler(ICourseRepository courseRepository, IRepository<User> userRepository, IRepository<UserCourseRelationship> userCourseRelationshipRepository)
        {
            _courseRepository = courseRepository;
            _userRepository = userRepository;
            _userCourseRelationshipRepository = userCourseRelationshipRepository;
        }

        public async Task ExecuteAsync(CreateCourseCommand message, CancellationToken token)
        {
            var user = await _userRepository.LoadAsync(message.UserId, token);
            var course = await _courseRepository.GetCourseAsync(user.University.Id, message.Name, token);

            if (course == null)
            {
                course = new Course(message.Name, user.University);
                await _courseRepository.AddAsync(course, token).ConfigureAwait(true);
            }
            var relationship = new UserCourseRelationship(user,course);
            await _userCourseRelationshipRepository.AddAsync(relationship, token);
        }
    }
}