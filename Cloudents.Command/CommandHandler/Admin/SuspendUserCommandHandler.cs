using Cloudents.Command.Command.Admin;
using Cloudents.Core.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.CommandHandler.Admin
{
    public class SuspendUserCommandHandler : ICommandHandler<SuspendUserCommand>
    {
        private readonly IRegularUserRepository _userRepository;


        public SuspendUserCommandHandler(IRegularUserRepository userRepository)
        {
            _userRepository = userRepository;
        }


        public async Task ExecuteAsync(SuspendUserCommand message, CancellationToken token)
        {
            var user = await _userRepository.LoadAsync(message.Id,  token);

            user.SuspendUser(message.LockoutEnd);


            if (message.ShouldDeleteData)
            {
                foreach (var question in user.Questions)
                {
                    question.DeleteQuestionAdmin();
                }
                foreach (var answer in user.Answers)
                {
                    answer.Question.RemoveAnswer(answer, true);
                    //answer.DeleteAnswerAdmin();
                }

                user.DeleteQuestionAndAnswers();
            }
            await _userRepository.UpdateAsync(user, token);
        }
    }
}
