using System.Threading;
using System.Threading.Tasks;
using Cloudents.Command.Command.Admin;
using Cloudents.Core.Event;
using Cloudents.Core.Interfaces;

namespace Cloudents.Command.CommandHandler.Admin
{
    public class SuspendUserCommandHandler : ICommandHandler<SuspendUserCommand>
    {
        private readonly IRegularUserRepository _userRepository;
        private readonly IEventStore _eventStore;


        public SuspendUserCommandHandler(IRegularUserRepository userRepository,
            IEventStore eventStore)
        {
            _userRepository = userRepository;
            _eventStore = eventStore;
        }


        public async Task ExecuteAsync(SuspendUserCommand message, CancellationToken token)
        {
            var user = await _userRepository.LoadAsync(message.Id, false, token);
            //TODO: why????
            //if (!user.LockoutEnabled)
            //{
            //    return;
            //}
            user.LockoutEnd = message.LockoutEnd;
            _eventStore.Add(new UserSuspendEvent(user));


            if (message.ShouldDeleteData)
            {

                //    var deleteQuestionCommandHandler = _lifetimeScope.Resolve<DeleteQuestionCommandHandler>();

                //    foreach (var question in user.QuestionsReadOnly)
                //    {
                //        await deleteQuestionCommandHandler.DeleteQuestionAsync(question, user, token);
                //    }


                //    var deleteAnswerCommandHandler = _lifetimeScope.Resolve<DeleteAnswerCommandHandler>();
                //    foreach (var answer in user.AnswersReadOnly)
                //    {
                //        await deleteAnswerCommandHandler.DeleteAnswerAsync(answer, token);
                //    }

                user.DeleteQuestionAndAnswers();
            }
            await _userRepository.UpdateAsync(user, token);
        }
    }
}
