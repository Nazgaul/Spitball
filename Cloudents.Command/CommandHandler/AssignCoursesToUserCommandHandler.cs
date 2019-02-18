using Cloudents.Command.Command;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using System.Diagnostics.CodeAnalysis;
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

            //var firstCourseTransaction = await _transactionRepository.GetFirstCourseTransaction(message.UserId, token);

            //if (!user.Courses.Any() && firstCourseTransaction == TransactionActionType.None)
            //{
            //    user.AwardMoney(AwardsTransaction.FirstCourse);
            //    await _userRepository.UpdateAsync(user, token);
            //}
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

            await _userRepository.UpdateAsync(user, token);
        }
    }
}