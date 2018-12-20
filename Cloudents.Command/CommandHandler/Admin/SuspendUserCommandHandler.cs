using System.Threading;
using System.Threading.Tasks;
using Cloudents.Application.Command.Admin;
using Cloudents.Application.Event;
using Cloudents.Application.Interfaces;

namespace Cloudents.Application.CommandHandler.Admin
{
    public class SuspendUserCommandHandler : ICommandHandler<SuspendUserCommand>
    {
        private readonly IRegularUserRepository _userRepository;
        //private readonly ILifetimeScope _lifetimeScope;
        private readonly IEventStore _eventStore;


        public SuspendUserCommandHandler(IRegularUserRepository userRepository,
            //ILifetimeScope lifetimeScope,
            IEventStore eventStore)
        {
            _userRepository = userRepository;
            //_lifetimeScope = lifetimeScope;
            _eventStore = eventStore;
        }


        public async Task ExecuteAsync(SuspendUserCommand message, CancellationToken token)
        {
            var user = await _userRepository.LoadAsync(message.Id, false, token);
            //TODO: why????
            if (!user.LockoutEnabled)
            {
                return;
            }
            user.LockoutEnd = message.LockoutEnd;
            _eventStore.Add(new UserSuspendEvent(user));
            //user.Events.Add(new UserSuspendEvent(user));


            //if (message.ShouldDeleteData)
            //{

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
            //}
           // user.Questions.Clear();
            //user.Answers.Clear();
            await _userRepository.UpdateAsync(user, token);
        }
    }
}
