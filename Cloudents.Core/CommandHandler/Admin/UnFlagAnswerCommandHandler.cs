using Cloudents.Core.Command.Admin;
using Cloudents.Domain.Entities;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Domain.Enums;

namespace Cloudents.Core.CommandHandler.Admin
{
    public class UnFlagAnswerCommandHandler : ICommandHandler<UnFlagAnswerCommand>
    {
        private readonly IRepository<Answer> _answerRepository;

        public UnFlagAnswerCommandHandler(IRepository<Answer> answerRepository)
        {
            _answerRepository = answerRepository;
        }
        public async Task ExecuteAsync(UnFlagAnswerCommand message, CancellationToken token)
        {
            foreach (var answerId in message.AnswerIds)
            {
                var answer = await _answerRepository.LoadAsync(answerId, token);
                answer.Item.State = ItemState.Ok;
                answer.Item.FlagReason = null;
                answer.Item.Votes.Clear();

                await _answerRepository.UpdateAsync(answer, token);
            }

        }
    }
}
