﻿using Cloudents.Command.Command.Admin;
using Cloudents.Core.Event;
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
            var user = await _userRepository.LoadAsync(message.Id, false, token);
            //TODO: why????
            //if (!user.LockoutEnabled)
            //{
            //    return;
            //}
            user.LockoutEnd = message.LockoutEnd;
            user.Events.Add(new UserSuspendEvent(user));


            if (message.ShouldDeleteData)
            {

                //    var deleteQuestionCommandHandler = _lifetimeScope.Resolve<DeleteQuestionCommandHandler>();

                foreach (var question in user.Questions)
                {
                    question.DeleteQuestionAdmin();
                    //        await deleteQuestionCommandHandler.DeleteQuestionAsync(question, user, token);
                }


                //    var deleteAnswerCommandHandler = _lifetimeScope.Resolve<DeleteAnswerCommandHandler>();
                foreach (var answer in user.Answers)
                {
                    answer.DeleteAnswerAdmin();
                    //        await deleteAnswerCommandHandler.DeleteAnswerAsync(answer, token);
                }

                user.DeleteQuestionAndAnswers();
            }
            await _userRepository.UpdateAsync(user, token);
        }
    }
}