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
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Course> _courseRepository;
        private readonly IRepository<UserCourseRelationship> _userCourseRelationshipRepository;

        public AssignCourseToUserCommandHandler(IRepository<User> userRepository, IRepository<Course> courseRepository, IRepository<UserCourseRelationship> userCourseRelationshipRepository)
        {
            _userRepository = userRepository;
            _courseRepository = courseRepository;
            _userCourseRelationshipRepository = userCourseRelationshipRepository;
        }

        public async Task ExecuteAsync(AssignCourseToUserCommand message, CancellationToken token)
        {
            var user = await _userRepository.LoadAsync(message.UserId, token);
            var course = await _courseRepository.LoadAsync(message.CourseId, token);
            var relationship = new UserCourseRelationship(user, course);
            await _userCourseRelationshipRepository.AddAsync(relationship, token);
        }
    }
}