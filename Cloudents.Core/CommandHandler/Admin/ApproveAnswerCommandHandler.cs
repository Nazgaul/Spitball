using Cloudents.Core.Command.Admin;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Enum;
using Cloudents.Core.Event;
using Cloudents.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.CommandHandler.Admin
{
    class ApproveAnswerCommandHandler : ICommandHandler<ApproveAnswerCommand>
    {
        private readonly IRepository<Answer> _answerRepository;

        public ApproveAnswerCommandHandler(IRepository<Answer> answerRepository)
        {
            _answerRepository = answerRepository;
        }

        public async Task ExecuteAsync(ApproveAnswerCommand message, CancellationToken token)
        {
            foreach (var answerId in message.AnswerIds)
            {
                var answer = await _answerRepository.LoadAsync(answerId, token);
                answer.Item.State = ItemState.Ok;
                
                answer.Events.Add(new AnswerCreatedEvent(answer));
                await _answerRepository.UpdateAsync(answer, token);
            }

        }
    }
}
