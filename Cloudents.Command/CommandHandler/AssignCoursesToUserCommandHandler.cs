using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Command.Command;
using Cloudents.Core;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;

namespace Cloudents.Command.CommandHandler
{
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Ioc inject")]
    public class AssignCoursesToUserCommandHandler : ICommandHandler<AssignCoursesToUserCommand>
    {
        private readonly ICourseRepository _courseRepository;
        private readonly IRepository<RegularUser> _userRepository;
        private readonly ITransactionRepository _transactionRepository;

        public AssignCoursesToUserCommandHandler(ICourseRepository courseRepository, 
            IRepository<RegularUser> userRepository, ITransactionRepository transactionRepository)
        {
            _courseRepository = courseRepository;
            _userRepository = userRepository;
            _transactionRepository = transactionRepository;
        }


        public async Task ExecuteAsync(AssignCoursesToUserCommand message, CancellationToken token)
        {
            var user = await _userRepository.LoadAsync(message.UserId, token);
            //TODO fix that.
            var firstCourseTransaction = user.Transactions.Where(w => w.Action == TransactionActionType.FirstCourse)
                                                            .Select(s => s.Action).FirstOrDefault();
            if (!user.Courses.Any() && firstCourseTransaction == TransactionActionType.None)
            {
                var t = new Transaction(TransactionActionType.FirstCourse, TransactionType.Earned, ReputationAction.FirstCourse, user);
                await _transactionRepository.AddAsync(t, token);
            }
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