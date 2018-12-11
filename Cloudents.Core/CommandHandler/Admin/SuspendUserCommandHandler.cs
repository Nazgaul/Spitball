﻿using Autofac;
using Cloudents.Core.Command.Admin;
using Cloudents.Core.Event;
using Cloudents.Core.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.CommandHandler.Admin
{
    public class SuspendUserCommandHandler : ICommandHandler<SuspendUserCommand>
    {
        private readonly IRegularUserRepository _userRepository;
        private readonly ILifetimeScope _lifetimeScope;


        public SuspendUserCommandHandler(IRegularUserRepository userRepository, ILifetimeScope lifetimeScope)
        {
            _userRepository = userRepository;
            _lifetimeScope = lifetimeScope;
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
            user.Events.Add(new UserSuspendEvent(user));
            //await _userRepository.UpdateAsync(user, token);


            if (message.ShouldDeleteData)
            {

                var deleteQuestionCommandHandler = _lifetimeScope.Resolve<DeleteQuestionCommandHandler>();

                foreach (var question in user.Questions)
                {
                    await deleteQuestionCommandHandler.DeleteQuestionAsync(question, user, token);
                }


                var deleteAnswerCommandHandler = _lifetimeScope.Resolve<DeleteAnswerCommandHandler>();
                foreach (var answer in user.Answers)
                {
                    await deleteAnswerCommandHandler.DeleteAnswerAsync(answer, token);
                }
            }
            user.Questions.Clear();
            user.Answers.Clear();
            await _userRepository.UpdateAsync(user, token);
        }
    }
}
